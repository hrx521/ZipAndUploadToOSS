static class Status
{
    const char _block = '■';
    const string _back = "\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b";
    const string _twirl = "-\\|/";
    /// <summary>
    /// 进度条，用来报告确定的工作量当前完成百分比
    /// </summary>
    /// <param name="percent">进度，它是介于0和100之间（包括0和100）的数字</param>
    /// <param name="update">在第一次调用该方法时应为false，在随后的时间应为true</param>
    public static void WriteProgressBar(int percent, bool update = false)
    {
        if (update)
            Console.Write(_back);
        Console.Write("[");
        var p = (int)((percent / 10f) + .5f);
        for (var i = 0; i < 10; ++i)
        {
            if (i >= p)
                Console.Write(' ');
            else
                Console.Write(_block);
        }
        Console.Write("] {0,3:##0}%", percent);
    }
    /// <summary>
    /// 报告进度用来报告未知工作量，也叫开放式进度
    /// </summary>
    /// <param name="progress">只是一个整数值，每次都会递增</param>
    /// <param name="update">在第一次调用该方法时应为false，在随后的时间应为true</param>
    public static void WriteProgress(int progress, bool update = false)
    {
        if (update)
            Console.Write("\b");
        Console.Write(_twirl[progress % _twirl.Length]);
    }

    #region 此演示代码应演示：
    public static void Demo()
    {
        Status.WriteProgressBar(0);
        for (var i = 0; i <= 100; ++i)
        {
            Status.WriteProgressBar(i, true);
            Thread.Sleep(50);
        }
        Console.WriteLine();

        Status.WriteProgress(0);
        for (var i = 0; i <= 100; ++i)
        {
            Status.WriteProgress(i, true);
            Thread.Sleep(50);
        }
    }

    #endregion
}

