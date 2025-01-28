using System;
using System.Windows.Media;

namespace SimpleDataTemplateSample {
  public class Book {
    private string title;
    private string author;
    private Uri imageUri;

    public Book() { }
    public Book(string title, string author, Uri imageUri) {
      this.title = title;
      this.author = author;
      this.imageUri = imageUri;
    }
    public string Title {
      get { return this.title; }
      set { this.title = value; }
    }
    public string Author {
      get { return this.author; }
      set { this.author = value; }
    }
    public Uri ImageUri {
      get { return this.imageUri; }
      set { this.imageUri = value; }
    }
    public override string ToString() {
      return this.title + ", by " + this.author;
    }
  }
}
