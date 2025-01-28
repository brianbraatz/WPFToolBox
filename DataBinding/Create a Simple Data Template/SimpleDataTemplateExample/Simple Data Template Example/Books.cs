using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace SimpleDataTemplateSample {
  public class Books : ObservableCollection<Book> {
    public Books() {
      Add(new Book("Programming Windows Forms 2.0", "Chris Sells and Michael Weinhardt", new Uri("winforms20.jpg", UriKind.Relative)));
      Add(new Book("Programming Windows", "Charles Petzold", new Uri("programmingwindows.jpg", UriKind.Relative)));
      Add(new Book("Programming Windows Presentation Foundation", "Chris Sells and Ian Griffiths", new Uri("programmingwpf.jpg", UriKind.Relative)));
    }
  }
}