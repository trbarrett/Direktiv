module Direktiv.AWS

open Amazon
open Amazon.Lambda
open Amazon.Lambda.Model
open Amazon.Runtime.CredentialManagement

open System.Text.Json.Serialization
open System.Text.Json
open System.Text.Json.Nodes

open Direktiv

let loadProfiles () =
    let chain = CredentialProfileStoreChain()
    chain.ListProfiles() |> Seq.toList

let getCredentials profileName =
    let chain = CredentialProfileStoreChain()
    match chain.TryGetAWSCredentials profileName with
    | true, credentials -> credentials
    | false, _ -> failwithf "Could not load credentials for profileName"

let tryPrettyPrintJson str =
    let inline tryDeserialize (jsonStr : string) : JsonDocument option =
        try
            JsonSerializer.Deserialize(jsonStr) |> Some
        with ex ->
            None

    match tryDeserialize str with
    | Some x ->
        let json = JsonSerializer.Serialize(x, JsonSerializerOptions (WriteIndented = true))
        x.Dispose()
        json
    | None -> str


let invoke (profileName : string) (region : AwsRegion) functionName requestBody = async {
    printfn "Starting Lambda Request"
    let sw = System.Diagnostics.Stopwatch.StartNew()
    try
        let region = RegionEndpoint.GetBySystemName (AwsRegion.systemName region)
        use client = new AmazonLambdaClient(getCredentials profileName, region)
        let! response =
            InvokeRequest(
                FunctionName = functionName, Payload = requestBody, InvocationType = InvocationType.RequestResponse)
            |> client.InvokeAsync
            |> Async.AwaitTask

        sw.Stop()

        let responseText =
            response.Payload.ToArray()
            |> System.Text.Encoding.UTF8.GetString
            |> tryPrettyPrintJson

        printfn $"Got Response Text. Took {sw.ElapsedMilliseconds}ms"
        printfn $"{responseText}"

        return responseText, sw.Elapsed
    with ex ->
        return $"ERROR: {ex.Message}", sw.Elapsed
}
