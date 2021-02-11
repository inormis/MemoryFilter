using MemoryFilter.ViewModel;

namespace MemoryFilter {

    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();
        }

        private MainViewModel ViewModel => (MainViewModel) DataContext;
    }

}