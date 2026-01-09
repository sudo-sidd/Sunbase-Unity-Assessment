using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// Clean data model we use in our UI after merging API data.
/// This is what gets passed around the app - much easier to work with!
/// </summary>
public class ClientProfile
{
    public int id;
    public string label;      // From "clients" list
    public bool isManager;    // From "clients" list
    public string name;       // From "data" dictionary
    public string address;    // From "data" dictionary
    public int points;        // From "data" dictionary
}

/// <summary>
/// Matches the EXACT structure of the API JSON response.
/// The API returns: { "clients": [...], "data": {...}, "label": "..." }
/// 
/// IMPORTANT: Field names must match JSON keys exactly, or use [JsonProperty("keyName")]
/// </summary>
public class RootApiResponse
{
    public List<RawClient> clients;
    
    // This is a Dictionary because the API returns: "data": { "1": {...}, "2": {...} }
    // The keys are client IDs as strings, values are the detail objects
    public Dictionary<string, RawClientDetails> data; 
    public string label;
}

/// <summary>
/// Represents one item in the "clients" array from API
/// </summary>
public class RawClient
{
    public bool isManager;
    public int id;
    public string label;
}

/// <summary>
/// Represents the detail object inside "data" dictionary
/// Example: "data": { "1": { "address": "...", "name": "...", "points": 100 } }
/// </summary>
public class RawClientDetails
{
    public string address;
    public string name;
    public int points;
}