using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Trabo
{
    public class TaskRepeatObservable
    {
        public IObservable<T> Create<T>(Func<Task<T>> funcOfTask, CancellationToken cancellationToken)
        {
            Subject<T> subject = new Subject<T>();
            Task.Run(() => Run(subject, funcOfTask, cancellationToken));
            return subject;
        }

        private async void Run<T>(Subject<T> subject, Func<Task<T>> funcOfTask, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await funcOfTask();
                subject.OnNext(result);
            }

            subject.OnCompleted();
        }
    }
}