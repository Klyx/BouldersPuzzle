using UnityEditor;

namespace FG {
	[InitializeOnLoad]
	public class PreloadSigningAlias {
		static PreloadSigningAlias ()
		{
			PlayerSettings.Android.keystorePass = "mc@5AJvGnCIwG!g&&suuPr4ILu#tq9SOf461B34rcYFcTIm2tO";
			PlayerSettings.Android.keyaliasName = "boulderspuzzle";
			PlayerSettings.Android.keyaliasPass = "hJmUK&puQ0p0eI$mtkRwe4rVOGqdz02GpiDYfPmIe4DQ@2OKbG";
		}
	}
}
