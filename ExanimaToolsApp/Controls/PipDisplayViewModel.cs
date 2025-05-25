using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ExanimaTools.Controls
{
    public enum PipState { Empty, Half, Full }

    public partial class PipDisplayViewModel : ObservableObject
    {
        public ObservableCollection<PipState> Pips { get; } = new();
        public void SetPips(int full, bool half, int max)
        {
            Pips.Clear();
            for (int i = 0; i < full; i++) Pips.Add(PipState.Full);
            if (half) Pips.Add(PipState.Half);
            for (int i = full + (half ? 1 : 0); i < max; i++) Pips.Add(PipState.Empty);
        }
    }
}
