module Direktiv.AWS

open Amazon
open Amazon.Lambda
open Amazon.Lambda.Model
open Amazon.Runtime.CredentialManagement

open Direktiv

let loadProfiles () =
    let chain = CredentialProfileStoreChain()
    chain.ListProfiles() |> Seq.toList

let getCredentials profileName =
    let chain = CredentialProfileStoreChain()
    match chain.TryGetAWSCredentials profileName with
    | true, credentials -> credentials
    | false, _ -> failwithf "Could not load credentials for profileName"

let invoke (profileName : string) (region : AwsRegion) functionName requestBody = async {
    printfn "Starting Lambda Request"
    let region = RegionEndpoint.GetBySystemName (AwsRegion.systemName region)
    use client = new AmazonLambdaClient(getCredentials profileName, region)
    let! response =
        InvokeRequest(
            FunctionName = functionName, Payload = requestBody, InvocationType = InvocationType.RequestResponse)
        |> client.InvokeAsync
        |> Async.AwaitTask

    let responseText =
        response.Payload.ToArray()
        |> System.Text.Encoding.UTF8.GetString

    printfn "Got Response Text"
    printfn $"{responseText}"

    return responseText
}
