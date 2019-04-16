// Learn more about F# at http://fsharp.org

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe

[<CLIMutable>] //Compiles to native .net object with getter/setter
type Radio =
  {
    id: int
    alias: string
    allowed_locations: string list
    location: Option<string>
  }

let addRadio (id: int): HttpHandler =
  fun (next: HttpFunc) (ctx: HttpContext) ->
    task {
      let! radio = ctx.BindJsonAsync<Radio>()

      // TODO check for empty fiels on alias and allowed_locations
      return! Successful.OK { radio with id = id } next ctx
      //return! text (sprintf "adding radio %d" id) next ctx
    }


let webApp =
  choose [
    POST >=> routef "/radios/%i" addRadio
  ]

let configureApp (app: IApplicationBuilder) = app.UseGiraffe webApp
let configureServices (services: IServiceCollection) =
  services.AddGiraffe() |> ignore

[<EntryPoint>]
let main _ =
  WebHostBuilder()
    .UseKestrel()
    .Configure(Action<IApplicationBuilder> configureApp)
    .ConfigureServices(configureServices)
    .UseUrls("http://*:8888")
    .Build()
    .Run()
  0
