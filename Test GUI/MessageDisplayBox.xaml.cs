namespace Test_GUI
{
    /// <summary>
    /// Interaction logic for MessageDisplayBox.xaml
    /// </summary>
    public partial class MessageDisplayBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDisplayBox"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        public MessageDisplayBox(string content, string title="")
        {
            InitializeComponent();
            txtContent.Text = content;
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;
        }
    }
}
