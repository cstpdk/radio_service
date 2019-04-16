module Main

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe
open StackExchange.Redis
open Endpoints


let webApp =
  choose [
    POST >=> routef "/radios/%i" addRadio
    POST >=> routef "/radios/%i/location" changeLocation
    GET >=> routef "/radios/%i/location" getLocation
  ]

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An exception has occurred while executing the request.")
    clearResponse
    >=> ServerErrors.INTERNAL_ERROR ex.Message

let configureApp (app: IApplicationBuilder) =
  app
    .UseGiraffeErrorHandler(errorHandler)
    .UseGiraffe webApp

let configureServices (services: IServiceCollection) =
  services.AddGiraffe() |> ignore
  services.AddSingleton<Radio.DB>(Radio.newDB()) |> ignore

let configureLogging (builder: ILoggingBuilder) =
  let filter (l: LogLevel) = l.Equals LogLevel.Error
  builder
    .AddFilter(filter)
    .AddConsole()
    .AddDebug() |> ignore


[<EntryPoint>]
let main _ =
  WebHostBuilder()
    .UseKestrel()
    .Configure(Action<IApplicationBuilder> configureApp)
    .ConfigureServices(configureServices)
    .ConfigureLogging(configureLogging)
    .UseUrls("http://*:8888")
    .Build()
    .Run()
  0
