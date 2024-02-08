# Codename: DarkBot
This is a very cool custom Discord bot made using Blazor & DSharp+ fo server called Chillec
## Project Architecture
![image](https://github.com/TheDarkun/DarkBot/assets/106868917/d99ecd30-54ed-40fb-b69b-74020139b3f6)
## How to contribute
There is no need to be good at programming, you can just put up a decentish issue here https://github.com/TheDarkun/DarkBot/issues. Of course if you are a compenent programmer, you can assign one issue to yourself (feature, bugfix, etc.) and then create your own branch with prefix such as "feature/"
### Naming Convention
When you are programming you have to try and comply with rules such as naming convention. Having coding standards and best practices are important!
| Object Name               | Notation   |
|:--------------------------|:-----------|
| Namespace name            | PascalCase |
| Class name                | PascalCase |
| Constructor name          | PascalCase |
| Method name               | PascalCase |
| Method arguments          | camelCase  |
| Local variables           | camelCase  |
| Constants name            | PascalCase |
| Field name Public         | PascalCase |
| Field name Private        | camelCase |
| Properties name           | PascalCase |
| Delegate name             | PascalCase |
| Enum type name            | PascalCase |
### Project Structure
Also please try to respect file organization.

![image](https://github.com/TheDarkun/DarkBot/assets/106868917/ac52ec8b-d8e5-44a5-8b0a-c81db6202994)
### secrets.json
Instead of putting variables in appSettings.json, use secrets.json. This prevents you from accidentally commiting your tokens.
```json
{
  "randomJwtToken": "",
  "botToken": "",
  "redirectURI": "",
  "clientId": "",
  "clientSecret" : "",
  "baseURI": "",
  "apiVersion": ""
}
```
- randomJwtToken
  - it's a random sequence of characters, this is used for creating secure Jwt Tokens
  - for example: "sv2)Wp7FW@fwX45N+%$Q?[Er8r6[272+zUPMjB?25c#f53Qkfc!fgEvCwUsDfbC&" ~~don't use this one~~
- botToken
  - this is your typical discord bot token which you can get [here](https://discord.com/developers/applications)
  - for example: "XDDwMzcxNballs9768A68gypfa3OA.aEvGHxP.0PibRfDhdemN-UDTC4enm1HAyPaf-Y-PFL8fpO" ~~also don't use this one~~
- redirectURI
  - in the **OAuth2** tab, create your redirect link
    - use the same one you have in **apiVersion**, but just include "api/Account/Authenticate"
    - for example: "https://localhost:5000/api/Account/Authenticate" ~~you **can** actually use this~~
  - after creating your redirect, go to **URL Generator** and create you link here (don't forget to select the correct redirect link)
    - in Scopes apply:
      1. identify 
- clientId
  - you can find this in the **OAuth2** tab
  - for example: "1686817684079826878" ~~really why would you use this one~~
- clientSecret
  - it's right next to the clientId, except you have to reset it
  - for example: "?[Er8r6[272+zUPMjB?25c#f53Qkfc!fgEvCwU" ~~don't even bother using this one~~
- baseURI
  - this is where you host your app!
  - for example: "https://localhost:5000/" ~~you **can** actually use this~~
- apiVersion
  - discord api keeps evolving and it wouldn't be productive to have issues 24/7 (unlike on your typical discord app), you can specify the version of your api
  - you can find all the versions [here](https://discord.com/developers/docs/reference)
  - for example: "10" ~~probably use this one~~   
