using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using TabbyCat.App.Interfaces;
using TabbyCat.App.Interfaces.Impls;

namespace TabbyCat.App;

public sealed class ClipboardWatcher : LoopClipboardWatcherBase;