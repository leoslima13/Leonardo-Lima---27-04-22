using System;
using System.Collections.Generic;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace TaskList.Extensions
{
    public static class ExtensionsOfReactiveCommand
    {
        public static ReactiveCommand WithSubscribeDisposing(this ReactiveCommand command, Action onNext, ICollection<IDisposable> disposables)
        {
            return command.WithSubscribe(onNext, x => x.AddTo(disposables));
        }
    }
}