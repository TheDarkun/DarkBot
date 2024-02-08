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
### Real programming
When you finally realize, creating a discord bot is your passion, you can start... but there are some rules.
1. create an Issue [here](https://github.com/TheDarkun/DarkBot/issues)
  - use English!
  - try making sense!
  - create proper **prerequisites**
    1. Create a simple tl;dr for your problem/idea
    2. Try to find possible solutions (only if you know what you're doing)
    3. Ideally make a visualization or even a video
      - video is important for reproducing some bugs for example
      - even a simple wireframe or something in paint is great for others to understand your problem/idea
2. assign yourself to the Issue and create a branch there
- ![image](https://github.com/TheDarkun/DarkBot/assets/106868917/332340bb-be23-42f4-9bc9-4993f5c8447b)
  - please, create a branch through here, this is because when your pull request is successfull and you merge your branch, the issue automatically closes
  - creating branch also has its naming convention!
    - for feature use prefix **"feature/"**
      - for example: "feature/dark-mode"
    - for fixing a bug use prefix **"bugfix/"**
      - for example: "bugfix/aside-button"
3. create!
  - if you get to this point, you can program
  - **DON'T YOLO DEVELOP AND CREATE ONLY FEATURE/BUGFIX YOU ARE ASSIGNED TO DO**
    - when you find out, there might be something that enhances you code, create an issue with prerequisites again
  - please try and respect naming conventions and of course project structure!
  - ![image](https://github.com/TheDarkun/DarkBot/assets/106868917/ac52ec8b-d8e5-44a5-8b0a-c81db6202994)
4. pull request
  - after you are finished, you might want to merge your branch to main immediatelly
  - unfortunately you have to create a pull request first, this is because we don't want to add broken code into our main branch!
  - someone has to check your code and has to approve your pull request
5. merging
  - congratulations!!! you've made it
  - now you can enjoy your perfect code in main and can create a proper release!



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
  - in the **OAuth2** tab, create your redirect link (everything about OAuth2 is [here](https://discord.com/developers/applications))
    - use the same one you have in **baseURI**, but just include "api/Account/Authenticate"
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
