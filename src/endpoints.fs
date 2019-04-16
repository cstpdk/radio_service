module Endpoints

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe

let addRadio (id: int): HttpHandler =
  fun (next: HttpFunc) (ctx: HttpContext) ->
    let db = ctx.GetService<Radio.DB>();
    task { // Async context
      let! radio = ctx.BindJsonAsync<Radio.T>()

      return!
        // The BindJsonAsync leaks null for unrepresented fields. This
        // is not a "proper" F# type, so we need some box'ing in order to match
        if box radio.alias = null || box radio.allowed_locations = null then
          RequestErrors.UNPROCESSABLE_ENTITY
            "Please provide both alias and allowed_locations" next ctx
        else if Radio.save db id radio then
          Successful.OK { radio with id = id } next ctx
        else
          ServerErrors.INTERNAL_ERROR "Couldn't save radio" next ctx
    }


let changeLocation (id: int): HttpHandler =
  fun (next: HttpFunc) (ctx: HttpContext) ->
    let db = ctx.GetService<Radio.DB>();
    task {
      let! lc = ctx.BindJsonAsync<Radio.LocationChange>()

      return!
        match Radio.find db id with
        | None ->
          RequestErrors.NOT_FOUND "No such radio" next ctx
        | Some(radio) when not (List.contains lc.location radio.allowed_locations) ->
          RequestErrors.FORBIDDEN "location not allowed" next ctx
        | Some(radio) when Radio.save db id { radio with location = Some(lc.location) } ->
          Successful.OK "ok" next ctx
        | Some(_) ->
          ServerErrors.INTERNAL_ERROR "Couldn't save radio" next ctx
    }

let getLocation (id: int): HttpHandler =
  fun (next:HttpFunc) (ctx: HttpContext) ->
    let db = ctx.GetService<Radio.DB>();

    match Radio.find db id with
    | Some(radio) ->
      match radio.location with
      | Some(l) ->
        let newLocation: Radio.LocationChange = { location = l }
        Successful.OK newLocation next ctx
      | None -> RequestErrors.NOT_FOUND "No location on radio" next ctx
    | None -> RequestErrors.NOT_FOUND "No such radio" next ctx
