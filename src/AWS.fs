module Direktiv.AWS

open Amazon
open Amazon.Lambda
open Amazon.Lambda.Model

open Direktiv

let invoke (region : AwsRegion) functionName requestBody = async {
    printfn "Starting Lambda Request"
    let region = RegionEndpoint.GetBySystemName (AwsRegion.systemName region)
    use client = new AmazonLambdaClient(region)
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
