﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

public static class LoggerExtensions
{
    private static readonly Action<ILogger, Exception> _sleep = LoggerMessage.Define(
        LogLevel.Information,
        new EventId(1, nameof(Sleep)),
        "Have a nice dream!");
    private static readonly Action<ILogger, Food, Exception> _eat = LoggerMessage.Define<Food>(
        LogLevel.Information,
        new EventId(2, nameof(Eat)),
        "Eat (Food = {food}).");
    private static readonly Action<ILogger, IReadOnlyDictionary<string, object>, string, string, int, Exception> _ex = LoggerMessage.Define<IReadOnlyDictionary<string, object>, string, string, int>(
        LogLevel.Information,
        new EventId(3, nameof(LogEx)),
        "LogEx({obj}, {memberName}@{filePath}:{lineNumber}).");

    public static void Sleep(this ILogger logger)
    {
        _sleep(logger, null);
    }

    public static void Eat(this ILogger logger, Food food)
    {
        _eat(logger, food, null);
    }

    public static void LogEx(
        this ILogger logger,
        IReadOnlyDictionary<string, object> obj,
        [CallerMemberName] string memberName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        _ex(logger, obj, memberName, filePath, lineNumber, null);
    }
}
