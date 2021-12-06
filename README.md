# iNaturalist + Unity
iNaturalist + Unity is a third-party tool created by Josh Aaron Miller to provide integration between [Unity](https://unity.com/) projects and the [iNaturalist](https://www.inaturalist.org) API.

## Disclaimers
This repository is not endorsed by iNaturalist, the California Academy of Sciences, or National Geographic.

This repository is not sponsored by or affiliated with Unity Technologies or its affiliates. “Unity” is a trademark or registered trademark of Unity Technologies or its affiliates in the U.S. and elsewhere.

## Getting Started
1. Add this repository to your Unity project by downloading the code and moving it into your project or by adding it directly from the Unity Asset Store (coming soon!)
2. For any scripts that want to interact with iNaturalist, import this code by adding `using JoshAaronMiller.INaturalist;` at the top of the file.
3. Add an `INatManager` component to any GameObject, e.g.: `INatManager myINatManager = gameObject.AddComponent<INatManager>();` or by adding the component in the Unity editor.
4. Use the INatManager to make calls to the API following the documentation, see the examples below.

### General Usage Notes
All calls to the `INatManager` require two parameters: a function to callback when the request returns successfully, and a function to callback when the request fails.

Example:

```
INatManager iNatManager;
ObservationSearch myObservationSearch;
// ...

public void ProcessObservations(Results<Observations> myObservationResults){
// do stuff
}

public void HandleError(Error error){
// do stuff
}

iNatManager.SearchObservations(myObservationSearch, ProcessObservations, HandleError);
```


## Common Use Cases and Examples



## iNaturalist API Documentation
The official iNaturalist API documentation is available [here](https://api.inaturalist.org/v1/docs/).


## Documentation coming soon...
