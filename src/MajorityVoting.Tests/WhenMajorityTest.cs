// Copyright 2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Eduasync
{
    [TestFixture]
    public class WhenMajorityTest
    {
        [Test]
        public void NullSequenceOfTasks()
        {
            IEnumerable<Task<string>> tasks = null;
            Assert.Throws<ArgumentNullException>(() => MoreTaskEx.WhenMajority(tasks));
        }

        [Test]
        public void EmptySequenceOfTasks()
        {
            IEnumerable<Task<string>> tasks = new Task<string>[0];
            Assert.Throws<ArgumentException>(() => MoreTaskEx.WhenMajority(tasks));
        }

        [Test]
        public void NullReferencesWithinSequence()
        {
            // Create a task just so we'd *otherwise* be valid
            var timeMachine = new TimeMachine();
            var task = timeMachine.AddSuccessTask(1, "x");
            Assert.Throws<ArgumentException>(() => MoreTaskEx.WhenMajority(task, null));
        }

        [Test]
        public void SimpleSuccess()
        {
            var timeMachine = new TimeMachine();
            // All three tasks return the same value.
            var task1 = timeMachine.AddSuccessTask(1, "x");
            var task2 = timeMachine.AddSuccessTask(2, "x");
            var task3 = timeMachine.AddSuccessTask(3, "x");

            var resultTask = MoreTaskEx.WhenMajority(task1, task2, task3);
            Assert.IsFalse(resultTask.IsCompleted);

            // Only one result so far - no consensus
            timeMachine.AdvanceTo(1);
            Assert.IsFalse(resultTask.IsCompleted);

            // Second result gives a majority
            timeMachine.AdvanceTo(2);
            Assert.AreEqual(TaskStatus.RanToCompletion, resultTask.Status);
            Assert.AreEqual("x", resultTask.Result);
        }

        // This is the same as SimpleSuccess, except for the ordering of the arguments
        // to WhenMajority
        [Test]
        public void InputOrderIsIrrelevant()
        {
            var timeMachine = new TimeMachine();
            var task1 = timeMachine.AddSuccessTask(1, "x");
            var task2 = timeMachine.AddSuccessTask(2, "x");
            var task3 = timeMachine.AddSuccessTask(3, "x");

            var resultTask = MoreTaskEx.WhenMajority(task3, task2, task1);
            Assert.IsFalse(resultTask.IsCompleted);

            timeMachine.AdvanceTo(1);
            Assert.IsFalse(resultTask.IsCompleted);

            timeMachine.AdvanceTo(2);
            Assert.AreEqual(TaskStatus.RanToCompletion, resultTask.Status);
            Assert.AreEqual("x", resultTask.Result);
        }

        [Test]
        public void MajorityWithSomeDisagreement()
        {
            var timeMachine = new TimeMachine();
            // Second task gives a different result
            var task1 = timeMachine.AddSuccessTask(1, "x");
            var task2 = timeMachine.AddSuccessTask(2, "y");
            var task3 = timeMachine.AddSuccessTask(3, "x");

            var resultTask = MoreTaskEx.WhenMajority(task1, task2, task3);
            Assert.IsFalse(resultTask.IsCompleted);

            // Only one result so far - no consensus
            timeMachine.AdvanceTo(1);
            Assert.IsFalse(resultTask.IsCompleted);

            // Two results so far disagree
            timeMachine.AdvanceTo(2);
            Assert.IsFalse(resultTask.IsCompleted);

            // Third result gives majority verdict
            timeMachine.AdvanceTo(3);
            Assert.AreEqual(TaskStatus.RanToCompletion, resultTask.Status);
            Assert.AreEqual("x", resultTask.Result);
        }

        [Test]
        public void MajorityWithFailureTask()
        {
            var timeMachine = new TimeMachine();
            // Second task gives a different result
            var task1 = timeMachine.AddSuccessTask(1, "x");
            var task2 = timeMachine.AddFaultingTask<string>(2, new Exception("Bang!"));
            var task3 = timeMachine.AddSuccessTask(3, "x");

            var resultTask = MoreTaskEx.WhenMajority(task1, task2, task3);
            Assert.IsFalse(resultTask.IsCompleted);

            // Only one result so far - no consensus
            timeMachine.AdvanceTo(1);
            Assert.IsFalse(resultTask.IsCompleted);

            // Second result is a failure
            timeMachine.AdvanceTo(2);
            Assert.IsFalse(resultTask.IsCompleted);

            // Third result gives majority verdict
            timeMachine.AdvanceTo(3);
            Assert.AreEqual(TaskStatus.RanToCompletion, resultTask.Status);
            Assert.AreEqual("x", resultTask.Result);
        }

        [Test]
        public void EarlyFailure()
        {
            var timeMachine = new TimeMachine();
            // Second task gives a different result
            var task1 = timeMachine.AddCancelTask<string>(1);
            var task2 = timeMachine.AddFaultingTask<string>(2, new Exception("Bang 2!"));
            var task3 = timeMachine.AddSuccessTask(3, "x");
            var resultTask = MoreTaskEx.WhenMajority(task1, task2, task3);
            Assert.IsFalse(resultTask.IsCompleted);

            // First result is a cancellation
            timeMachine.AdvanceTo(1);
            Assert.IsFalse(resultTask.IsCompleted);

            // Second result is a failure. We now can't have a successful majority
            timeMachine.AdvanceTo(2);
            Assert.AreEqual(TaskStatus.Faulted, resultTask.Status);
        }

        [Test]
        public void NoMajority()
        {
            var timeMachine = new TimeMachine();
            // Second task gives a different result
            var task1 = timeMachine.AddSuccessTask(1, "x");
            var task2 = timeMachine.AddSuccessTask(2, "y");
            var task3 = timeMachine.AddSuccessTask(3, "z");

            var resultTask = MoreTaskEx.WhenMajority(task1, task2, task3);
            Assert.IsFalse(resultTask.IsCompleted);

            // Only one result so far - no consensus
            timeMachine.AdvanceTo(1);
            Assert.IsFalse(resultTask.IsCompleted);

            // Two results so far disagree
            timeMachine.AdvanceTo(2);
            Assert.IsFalse(resultTask.IsCompleted);

            // Third result is different again
            timeMachine.AdvanceTo(3);
            Assert.AreEqual(TaskStatus.Faulted, resultTask.Status);
        }
    }
}
