﻿// This file is part of Hangfire.
// Copyright © 2013-2014 Sergey Odinokov.
// 
// Hangfire is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// Hangfire is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with Hangfire. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;

namespace Hangfire.Client
{
    /// <summary>
    /// Provides information about the context in which the job
    /// is being created.
    /// </summary>
    public class CreateContext
    {
        private readonly IStateMachineFactory _stateMachineFactory;

        private readonly IDictionary<string, string> _parameters
            = new Dictionary<string, string>();

        private bool _jobWasCreated;

        internal CreateContext(CreateContext context)
            : this(context.Connection, context._stateMachineFactory, context.Job, context.InitialState)
        {
            Items = context.Items;
            JobId = context.JobId;
            _jobWasCreated = context._jobWasCreated;
            _parameters = context._parameters;
        }

        internal CreateContext(
            IStorageConnection connection,
            IStateMachineFactory stateMachineFactory,
            Job job,
            IState initialState)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (stateMachineFactory == null) throw new ArgumentNullException("stateMachineFactory");
            if (job == null) throw new ArgumentNullException("job");
            if (initialState == null) throw new ArgumentNullException("initialState");

            _stateMachineFactory = stateMachineFactory;

            Connection = connection;
            Job = job;
            InitialState = initialState;

            Items = new Dictionary<string, object>();
        }

        public IStorageConnection Connection { get; private set; }

        /// <summary>
        /// Gets an instance of the key-value storage. You can use it
        /// to pass additional information between different client filters
        /// or just between different methods.
        /// </summary>
        public IDictionary<string, object> Items { get; private set; }

        public string JobId { get; private set; }
        public Job Job { get; private set; }

        /// <summary>
        /// Gets the initial state of the creating job. Note, that
        /// the final state of the created job could be changed after 
        /// the registered instances of the <see cref="IElectStateFilter"/>
        /// class are doing their job.
        /// </summary>
        public IState InitialState { get; private set; }

        /// <summary>
        /// Sets the job parameter of the specified <paramref name="name"/>
        /// to the corresponding <paramref name="value"/>. The value of the
        /// parameter is being serialized to a JSON string.
        /// </summary>
        /// 
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// 
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null or empty.</exception>
        public void SetJobParameter(string name, object value)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");

            if (_jobWasCreated)
            {
                throw new InvalidOperationException("Could not set parameter for a created job.");
            }

            var serializedValue = JobHelper.ToJson(value);

            if (!_parameters.ContainsKey(name))
            {
                _parameters.Add(name, serializedValue);
            }
            else
            {
                _parameters[name] = serializedValue;
            }
        }

        /// <summary>
        /// Gets the job parameter of the specified <paramref name="name"/>
        /// if it exists. The parameter is being deserialized from a JSON 
        /// string value to the given type <typeparamref name="T"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value of the given parameter if it exists or null otherwise.</returns>
        /// 
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null or empty.</exception>
        /// <exception cref="NotSupportedException">Could not deserialize the parameter value to the type <typeparamref name="T"/>.</exception>
        public T GetJobParameter<T>(string name)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");

            try
            {
                return _parameters.ContainsKey(name)
                    ? JobHelper.FromJson<T>(_parameters[name])
                    : default(T);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format(
                    "Could not get a value of the job parameter `{0}`. See inner exception for details.",
                    name), ex);
            }
        }

        internal virtual void CreateJob()
        {
            var stateMachine = _stateMachineFactory.Create(Connection);

            JobId = stateMachine.CreateInState(Job, _parameters, InitialState);
            _jobWasCreated = true;
        }
    }
}