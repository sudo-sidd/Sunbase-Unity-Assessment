// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Networking;
// using Newtonsoft.Json;

// public class ClientService : MonoBehaviour
// {
//     private const string API_URL = "https://qa.sunbasedata.com/sunbase/portal/api/assignment.jsp?cmd=client_data";

//     public void FetchClients(Action<List<ClientProfile>> onSuccess, Action<string> onFailure)
//     {
//         StartCoroutine(GetRequest(onSuccess, onFailure));
//     }

//     private IEnumerator GetRequest(Action<List<ClientProfile>> onSuccess, Action<string> onFailure)
//     {
//         using (UnityWebRequest req = UnityWebRequest.Get(API_URL))
//         {
//             yield return req.SendWebRequest();

//             if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
//             {
//                 onFailure?.Invoke(req.error);
//             }
//             else
//             {
//                 ProcessData(req.downloadHandler.text, onSuccess, onFailure);
//             }
//         }
//     }

//     // Merging the two data sources
//     private void ProcessData(string json, Action<List<ClientProfile>> onSuccess, Action<string> onFailure)
//     {
//         try
//         {
            
//             RootApiResponse rawData = JsonConvert.DeserializeObject<RootApiResponse>(json);
            
//             if (rawData == null || rawData.clients == null) 
//             {
//                 onFailure?.Invoke("Data format invalid");
//                 return;
//             }

            
//             List<ClientProfile> cleanList = new List<ClientProfile>();

//             foreach (var rawClient in rawData.clients)
//             {
//                 ClientProfile profile = new ClientProfile();
//                 profile.id = rawClient.id;
//                 profile.isManager = rawClient.isManager;
//                 profile.label = rawClient.label;

//                 string idKey = rawClient.id.ToString();

//                 if (rawData.data != null && rawData.data.ContainsKey(idKey))
//                 {
//                     var details = rawData.data[idKey];
//                     profile.name = details.name;
//                     profile.address = details.address;
//                     profile.points = details.points;
//                 }
//                 else
//                 {
//                     profile.name = "Unknown";
//                     profile.points = 0;
//                 }

//                 cleanList.Add(profile);
//             }

//             onSuccess?.Invoke(cleanList);
//         }
//         catch (Exception e)
//         {
//             Debug.LogError($"Parsing Error: {e.Message}");
//             onFailure?.Invoke("Failed to parse data.");
//         }
//     }
// }
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

/// <summary>
/// Handles all API communication for client data.
/// This is a "Service" class - its only job is fetching and parsing data.
/// 
/// WHY MonoBehaviour? We need StartCoroutine() for async web requests.
/// </summary>
public class ClientService : MonoBehaviour
{
    // API endpoint - const because it never changes
    private const string API_URL = "https://qa.sunbasedata.com/sunbase/portal/api/assignment.jsp?cmd=client_data";

    /// <summary>
    /// Public method that the UI calls to get client data.
    /// Uses callbacks (Action) so the caller knows when data arrives or fails.
    /// 
    /// WHY callbacks? Because web requests are ASYNC - they take time.
    /// We can't just return the data, we have to notify when it's ready.
    /// </summary>
    public void FetchClients(Action<List<ClientProfile>> onSuccess, Action<string> onFailure)
    {
        StartCoroutine(GetRequest(onSuccess, onFailure));
    }

    /// <summary>
    /// Coroutine that actually makes the HTTP request.
    /// 
    /// WHY IEnumerator? Unity's web requests need coroutines to work asynchronously.
    /// The "yield return" pauses execution until the request completes.
    /// </summary>
    private IEnumerator GetRequest(Action<List<ClientProfile>> onSuccess, Action<string> onFailure)
    {
        Debug.Log("1. Starting API Request...");

        // "using" ensures the request is properly disposed (cleaned up) when done
        using (UnityWebRequest req = UnityWebRequest.Get(API_URL))
        {
            // This line PAUSES the coroutine until the web request finishes
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"2. API Failed: {req.error}");
                // The "?" is null-conditional - only calls Invoke if onFailure isn't null
                onFailure?.Invoke(req.error);
            }
            else
            {
                Debug.Log("2. API Success! Data received.");
                string json = req.downloadHandler.text;
                Debug.Log($"3. Raw JSON: {json}");
                ProcessData(json, onSuccess, onFailure);
            }
        }
    }

    /// <summary>
    /// Parses JSON and MERGES data from two different parts of the response.
    /// 
    /// The API returns data in two places:
    /// - "clients" array has: id, label, isManager
    /// - "data" dictionary has: name, address, points (keyed by id)
    /// 
    /// We combine them into one clean ClientProfile object per client.
    /// </summary>
    private void ProcessData(string json, Action<List<ClientProfile>> onSuccess, Action<string> onFailure)
    {
        try
        {
            Debug.Log("4. Attempting to parse JSON...");
            
            // JsonConvert.DeserializeObject<T>() converts JSON string to C# object
            // The type T (RootApiResponse) must match the JSON structure
            RootApiResponse rawData = JsonConvert.DeserializeObject<RootApiResponse>(json);
            
            // Defensive check - always validate API responses!
            if (rawData == null || rawData.clients == null) 
            {
                Debug.LogError("5. Parsed data is NULL or Clients list is missing!");
                onFailure?.Invoke("Data format invalid");
                return;
            }

            Debug.Log($"5. Found {rawData.clients.Count} clients in list.");

            List<ClientProfile> cleanList = new List<ClientProfile>();

            // Loop through each client and build a complete profile
            foreach (var rawClient in rawData.clients)
            {
                ClientProfile profile = new ClientProfile();
                
                // Copy basic info from "clients" array
                profile.id = rawClient.id;
                profile.isManager = rawClient.isManager;
                profile.label = rawClient.label;

                // MERGE: Look up additional details from "data" dictionary
                // The dictionary uses string keys, so convert id to string
                string idKey = rawClient.id.ToString();
                if (rawData.data != null && rawData.data.ContainsKey(idKey))
                {
                    var details = rawData.data[idKey];
                    profile.name = details.name;
                    profile.address = details.address;
                    profile.points = details.points;
                }
                
                cleanList.Add(profile);
            }

            Debug.Log($"6. Finished processing. Sending {cleanList.Count} profiles to UI.");
            onSuccess?.Invoke(cleanList);
        }
        catch (Exception e)
        {
            // Always wrap JSON parsing in try-catch - malformed data can crash the app!
            Debug.LogError($"CRITICAL PARSE ERROR: {e.Message}");
            onFailure?.Invoke("Failed to parse data.");
        }
    }
}