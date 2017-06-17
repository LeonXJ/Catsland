namespace Catslandx.Script.Exception {
  public class InvalidParameterException : System.Exception {

    public InvalidParameterException(string readableText)
      : base(readableText) {
    }
  }
}
