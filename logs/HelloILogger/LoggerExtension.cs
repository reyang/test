using System;
using Microsoft.Extensions.Logging;

public static class LoggerExtensions
{
    private static readonly Action<ILogger, Exception> _sleep;
    private static readonly Action<ILogger, Food, Exception> _eat;
    private static readonly Action<ILogger, string, int, Exception> _play;

    static LoggerExtensions()
    {
        _sleep = LoggerMessage.Define(LogLevel.Information, new EventId(1, nameof(Sleep)),  "Have a nice dream!");
        _eat = LoggerMessage.Define<Food>(LogLevel.Information, new EventId(2, nameof(Eat)), "Eat (Food = {food}).");
        _play = LoggerMessage.Define<string, int>(LogLevel.Information, new EventId(3, nameof(Play)), "Play (Game = {game}, Score = {score}).");
    }
    public static void Sleep(this ILogger logger)
    {
        _sleep(logger, null);
    }
    public static void Eat(this ILogger logger, Food food)
    {
        _eat(logger, food, null);
    }
    public static void Play(this ILogger logger, string game, int score)
    {
        _play(logger, game, score, null);
    }
}
