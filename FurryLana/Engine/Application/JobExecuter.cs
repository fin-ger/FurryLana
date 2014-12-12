using FurryLana.Engine.Graphics.Interfaces;
//
//  JobExecuter.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2014 Fin Christensen
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryLana.Engine.Application
{
    /// <summary>
    /// Job executer.
    /// </summary>
    public class JobExecuter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Base.Application.JobExecuter"/> class.
        /// </summary>
        public JobExecuter ()
        {
            Jobs = new List<Action> ();
        }

        /// <summary>
        /// Gets or sets the jobs.
        /// </summary>
        /// <value>The jobs.</value>
        public List<Action> Jobs
        {
            get
            {
                return jobs;
            }
            protected set
            {
                jobs = value;
            }
        }

        protected List<Action> jobs;

        /// <summary>
        /// Inserts the job.
        /// </summary>
        /// <param name="job">Job.</param>
        public void InsertJob (Action job)
        {
            lock (Jobs)
                Jobs.Add (job);
        }

        /// <summary>
        /// Inserts the jobs.
        /// </summary>
        /// <param name="jobs">Jobs.</param>
        public void InsertJobs (List<Action> jobs)
        {
            lock (Jobs)
                Jobs.AddRange (jobs);
        }

        /// <summary>
        /// Inserts the jobs.
        /// </summary>
        /// <param name="jobs">Jobs.</param>
        public void InsertJobs (Action[] jobs)
        {
            lock (Jobs)
                Jobs.AddRange (jobs);
        }

        /// <summary>
        /// Execs the jobs parallel.
        /// </summary>
        /// <param name="load">Load.</param>
        public void ExecJobsParallel (int load)
        {
            ParallelOptions ops = new ParallelOptions ();
            ops.MaxDegreeOfParallelism = load;
            lock (Jobs)
                Parallel.Invoke (ops, Jobs.ToArray ());
            Jobs.Clear ();
        }

        /// <summary>
        /// Execs the jobs sequential.
        /// </summary>
        public void ExecJobsSequential ()
        {
            lock (Jobs)
                Jobs.ForEach (a => a ());
            Jobs.Clear ();
        }

        /// <summary>
        /// Handler for a NeedsReexec event or delegate.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="args">Arguments.</param>
        public void NeedsReexecHandler (object action, EventArgs args)
        {
            Action act = action as Action;

            if (act == null)
                throw new ArgumentException ("Action should be of type Action!");

            lock (Jobs)
                Jobs.Add (act);
        }
    }
}
