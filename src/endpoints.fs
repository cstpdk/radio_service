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

      let radio = Radio.find db id

      let allowedLocations = radio.allowed_locations
      let newRadio = { radio with location = Some(lc.location) }
      return! (
        if not (List.contains lc.location allowedLocations) then
          RequestErrors.FORBIDDEN "location not allowed" next ctx
        else if Radio.save db id newRadio then
          Successful.OK "ok" next ctx
        else
          ServerErrors.INTERNAL_ERROR "Couldn't save radio" next ctx
      )
    }

let getLocation (id: int): HttpHandler =
  fun (next:HttpFunc) (ctx: HttpContext) ->
    let db = ctx.GetService<Radio.DB>();

    let radio = Radio.find db id

    match radio.location with
    | Some(l) ->
      let newLocation: Radio.LocationChange = { location = l }
      Successful.OK newLocation next ctx
    | None -> RequestErrors.NOT_FOUND "No location on radio" next ctx
