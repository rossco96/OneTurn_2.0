public static class ExtensionsString
{
	public static bool CanFormatCamelCase(this string s, out string sOut)
	{
		if (s[0].ToString().ToUpper() != s[0].ToString())
		{
			s = s[0].ToString().ToUpper() + s.Substring(1);
		}

		sOut = s;
		int capitalsCount = 0;

		// Go from i=1 to ignore the first capital
		for (int i = 1; i < s.Length; ++i)
		{
			if (s[i].ToString().ToUpper() == s[i].ToString())
			{
				sOut = sOut.Substring(0, i + capitalsCount) + " " + sOut.Substring(i + capitalsCount);
				capitalsCount++;
			}
		}

		return capitalsCount > 0;
	}

	public static long ToLong(this string s)
	{
		long returnValue = 0;
		for (int i = 0; i < s.Length; ++i)
		{
			int digit = int.Parse($"{s[s.Length - i - 1]}");
			returnValue += (long)(digit * UnityEngine.Mathf.Pow(10, i));
		}
		UnityEngine.Debug.Log($"string VS long\n{s}\n{returnValue}");
		return returnValue;
	}
}
