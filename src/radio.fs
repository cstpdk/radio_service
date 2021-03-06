[<RequireQualifiedAccess>]
module Radio

open StackExchange.Redis
open Newtonsoft.Json

// The redis package is meant for C#, this module is a small wrapper that
// helps it be more helpful to F#
module Redis =
  let getString (db:IDatabase) (k:string): Option<string> =
    let s = db.StringGet(RedisKey.op_Implicit k)
    if s.IsNullOrEmpty then
      None
    else
      Some(s |> RedisValue.op_Implicit)

  let setString (db:IDatabase) (k:string) (v:string): bool =
    db.StringSet(RedisKey.op_Implicit k, RedisValue.op_Implicit v)

[<CLIMutable>] //Compiles to native .net object with getter/setter
type T =
  {
    id: int
    alias: string
    allowed_locations: string list
    location: Option<string>
  }
[<CLIMutable>]
type LocationChange =
  {
    location: string
  }

let save (db:IDatabase) (id: int) (t:T) =
  Redis.setString db (sprintf "%d" id)
    <| JsonConvert.SerializeObject { t with id = id }

let find (db:IDatabase) (id: int): Option<T> =
  match sprintf "%d" id |> Redis.getString db with
  | None -> None
  | Some(a) -> Some(JsonConvert.DeserializeObject<T> a)

type DB = IDatabase
let newDB () = ConnectionMultiplexer.Connect("redis:6379").GetDatabase()
