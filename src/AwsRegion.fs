namespace Direktiv

type AwsRegion =
    | EuWest1
    | EuWest2

module AwsRegion =
    let description = function
        | EuWest1 -> "Europe (Ireland)"
        | EuWest2 -> "Europe (London)"

    let systemName = function
        | EuWest1 -> "eu-west-1"
        | EuWest2 -> "eu-west-2"

    let all = [ EuWest1; EuWest2 ]
