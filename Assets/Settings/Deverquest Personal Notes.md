At this point version 30.2 is the currents product to prepare for alpha and eventually shipment... first im butting us into beta right now

This will be a list of test data, as well as a list of thoughts for improvements / addons and bugs



### Release Preperation
- - ~~Changing the root folder name's because they still say code nazi here and there...~~
- - ~~moving the new read me from the suite to the actual root folder where it belongs~~
- - ~~placing the rest of the suite in the git root beside assets and everything ~~
- - ~~start tracking credits~~
- - - ChatGPT for pretty much all the coding
- - - RPG SOUNDS for warnings
- - - Sega SoundTracks for playlists
- - - loudlib for ambience sounds
- - - Confirmed the "Original Starter Identity" works fine, 
- - - - Guild Identity Registry -> Original Identity Catalog-> Ancestries, Classes, Faiths.-> all populated with good data. I love the unique classes and can't wait to add some more .I'm starting to put some lore together.



### Test Data

- Dinner notification popped up at the right time! thats nice. 
- I got a focus notification too and took the suggested break. 
- when we take the suggested break, I think it should tell us how long the suggestion is somewhere? unless it does and i just didnt see it. (Im pretty sure not taking the full break in quests affects my rewards in some way right) I found myself staring at the timer hoping I waited long enough but not actually taking a break in the end cuz i was just waiting out 80% of what i thought was a ten minute break... (I think we should also add a passive xp to just being in the app at all and not being idle...

- - im realizing it might be nice to have a dropdown to select the specific ambience track (same goes for the music too)
- - i notice the ambience persists when im in other screens but not the music, and when switching back into the unity editor i have to click pause and then play to start the music back up...

- - when i went to test a sound in the inspector something happened where i cant pause the music in Deverquesy now... (nor stop it) i think. When the song did complete tho controls returned to me like normal. The ambience durring all of this i think i actuall have had no control, and since its on loop i think its never ending and returning controls to me... I currently cant stop it or change it. And the warnings are not working now either...
- - I REALLY WISH I COULD GET AUDIO VOLUME CONTROLS WORKING, even if i have to build a system in the scene to make it work.
- why cant i put spaces in the Display name values in the data for alot of things (in that particular case im looking at the 'Display Name' value 'Project Name' and 'Task Name" values but im sure theresa bunch of others like that... it makes reading the quest names a little deceptive to the eye in the quest board...
- I've filled out a task and contract for 00_BetaTest, 01_FiveMinuteCallenge, and 02_OneHoureChallenge
- focus check-in worked, and progress report updated
- approved break has begun, i dont see anywhere that it tells me how long my break shouold be but it does warn me that any less than 80% wont count
### Improvements
## Quests-
# General
- Change terminology from focus to Predicted Task length and i think QuestContract is missleading too, Quest Task Profile makes more sense to me (we call our objectives tasks as a development team) and Quest Contract didn't read well before that change but i think it makes more sense now... maybe go as far as Quest Contract Profile?
- IMPORTANT RULE: Money can only be stacked on certain events... individual encounters / drops from single enemies cannot let the currency types convert, it needs to be converted in town at the money exchangeer or later certain store environments... 
- I dont see Reward and History reporting the daily decree completion, and next work block seems to remain zero, the coin purse and lifetime spoils did recive my 5 min task / quests updates... im not sure if a second report will appear hear later, but each quest's spoils individual reward should be displayed here too (just the rewards including items and whatnot...)

 

# Quest Profiles
- - Add a difficulty drop down to the Task profile. Signing up for harder difficulties has a base reward upon accepting (for being ballsy) and we can have a reward for completion im just now thinking (and maybe this will be a 2.0 and beyond addition, (at which point im calling a full scale expansion pack beyond the initial release.)) but the member accounts should have a seperate form of currency to spend outside of their characters, but THIS simoleons currency will be for the more real things like gift cars, boosts, time off, error forgiveness...
- - I'm thinking the difficulties them selves should be a data item so we can add tons of difficulties like diablo does where the have like nightmare x 20,
- - we'll use questy sounding names for difficulties encorporating the lore...
- Whimpy (Easy)
- Peasant Boy
- Bar Keep
- Adventurer (Normal)
- Knight (Hard)
- Veteran (Nightmare 1) - we should be able to come up with a better name for this (and maybe a few more predesesors) - im thinking we should come up with a name for this universe's hell (I'm trying to inject some programming terminoligy into the lore. I'm thinking gods are called like sigletons or something and The Sperk is a nod to a spark. I think hell would be like something relating to a crash or critical error but still with a fantasized motif)
- Veteran 2
- Veteran 3 - and so on

# Quest Contracts
- Contracts should also get a difficullty ranking (lets maybe change the name of priority to this lets make sure to keep draft and we need one doe disabled so we can have ones that dont automatically show up in the quest board...) to set having thier own set of rewards related directly to their character so each difficulty gets a base copper bonus (this could be a penalty for easier ranks) and an array of items rewarded so later nightmares can be the only way to farm things to upgrade certain items into certain teirs... gear locking people to their areas.
- priority can remain and let's let that organize their position in the board followed by dificulty. All the high priority will be at the top and all the high priority with have the harder difficulties at the bottom within their dificulty 
- I am not really sure how to update focus stages but like i've been saying I'd like this to be encounters... an encounter will be made up of several things, A biome, and structure type, outdoor, town, cabin, sewer, cave, castle, city streets, etc (as many as i can think of when the time comes), how many waves of enemies, how many enemies (and which) in a wave, (the combat will determing how long all of this SHOULD approx take to complete this many encounters) and an optional boss (enemies are optional too really). Chests if any(2.0 expansion), npcs if any (2.0 expansion) and we'll have "Additional" array for things like a buff alter, unconditional poison, scpecail crafting stations (2.0 expansion - I'll build on this idea later but if they complete an encounter with a crafting station or w/e i decided to call it, they will get some crafting advantage after the quest is completed)
- - this should all add up to an estimated encounter time...
(to be completly honest i have no idea how focused stages works right now in setup)
# Combat
 - - as mentioned im not really seeing any feed back about the actual combat so its toguh to say whats really going on there...
- I did see my spoils update at the focus check in

## Quest log
- I dont see completed quests here, i think they should be with a nice report of what happened in a collapsible object. and during a quest we should see a detailed report of everything actevely happening, wwe can include all the reporting the actual "Quest" tab does too with the Daily decree, Quest in progress timer, and progress bar / reporting...

## Git 
-I think the git stuff needs to all be isolated to its own tab



### Addons


## Visual Profiles
- - More color settings maybe profiles and even full controls panel to change any visuals and colors were allowed to (could be its own Visuals tab)
- - can we add images at all im thinking an icon for the characters, somthing the users can import themselves...
- - we should maybe be able to dock the timer and quest info on a seperate tab docked elsewhere from the rest of the panel? if thats possible, and also i think where it is tracking our decree and daily what not at the top, i think its should be displaying the text for whats happening in our quest...
- - another tab that just is a completed quests log...
- - - can we isolat all mod tools to a tab too?





### Bugs    -    DQ - 0302 - ###



-------------------------------------------
------An old comment i made which is now cevering in many of my notes here but i wanted to leave this for one more read before i remove it to consolidate the thought into the notes...
-------------------------------------------

also something i want to dial in befor i go any furthur and make things to complex to keep up with is all my items need a really detailed set of information, ontop of weight, they all need their appropriate type, meaning equipment, consumables, tradeskill supplies, lore books, merchant trash, and a ton of other things as we go i think, this specifically will link with a whole other section later on for crafting... 


i think im also thinking about biomes as a piece of data that can be attached to a quest that can  create different conditions
, maybe things like low movement in swamps but a bonus to earth type damage or something (we'll need to add tons of variables for this kind of thing, and maybe items related to each that can negate or increase the effects of each area if braught along... this will all tie into a larger system linking to the quest profiles where im starting to add on data that makes the quest have the story baked in by the pieces weve added... there could be something we add to a quest where were fighting in a burning area or building or whatever and we will all eventually die after it's engulfed, or a room that slowly poisons you, or maybe an alter we can attack that periodically heals people when they are in that room... 



im kind of actually thinking it could be like we attach our work data to it with the quest profile... we almost should rename these tasks because all my programmers will connect with that terminology better..., but as well as the task data they will get quest data, which iside gets an array of rooms, rooms can get data for how many monster are in each and whatnot (IF THEIRS A CHEST, trap, hazard, altar, traveling merchant etc... basically each room will have a target time in which we should be able to beat it's set up in, and the combination of rooms / areas (lets not forget a "room" can actually just be an outdoor area.. ) is what dictated the length of the quest (so the focuses or w/e were calling them now are actually more like combat encounters) and the overall configuration of the room is what dictates it's exp values and bonuses.. each little piece of data were adding to these overall quests, can also come along with a string detail or fact, that as a group together can be used as a sort of mad lib setup to print a quest story after the quest is completed ... the room can be a random arangement of "He entered a musty room", "They crawled into" the if we make it a cave it adds "dark troll cave" - which we set by filling in the bioms "detail" and "battled x "monsters" then give some type of rundown for the actual combat values "Ajnaag struck 2 skeletonXtimes for the kill and collected "A Skeletal finger bone", cloth scraps, and 22c" <- the items dropped by the individual (i cant thing of any great way to keep the actual battle 
details compact enough that its not just a swamps of attack this and attack that back and fourth maybe something like Ajnag killed x skeletons and collected x items and Peacebaby313 made the killing blow on a giant rat and collected a rat's tail, and uncooked rat meat" 

-------------------------------
END OF OLD COMMIT
--------------------------------








## Crafting Editions (2.0 Expansion)
 -the crafting system will come portionally by tradeskill group at a time and eventually expand a pretty expansive inventory, banking, and potentially housing system... 
 # Passive / Subtle Skills
 
 - Personal Skills
 - - Swimming
 - - Running
 - - Jumping
 - - Acrobatics (Maybe call it parkour)
 - - Grappling or Tumbling or Wrestling
 - - Throw
 - - Punch
 - - Kick
 - - Offense
 - - Defense
 - - Pick Pocketing
 - - Bribery
 - - Mercantile
 - - Painting
 - - Language Skills 
 - - (ill come up with more as we go)
 
 
 
 
 
 # Weapon Skills and Types
 - - Hand-to-hand
 - - Piercing
 - - 1H Sword
 - - 2H Sword
 - - 1H Axe
 - - 2H Axe
 - - 1H Mace
 - - 2H Mace
 - - 1H Hammer
 - - 2H Hammer
 - - Wand
 - - Stave
 - - Spear
 - - Halberd
 - - Bow
 - - Sling
 - - Block (Sheild)
 - - Unarmed Block
 
 
 - Threshing? (i can't think of anything better to call collecting cloth
 - - ClothWeaving
 - - - Tailoring
 - Mining
 - - Smelting
 - - - Blacksmithing
 - Skinning
 - - Tanning
 - - - Leather Working
 - Herbalism
 - - Alchemy
 - Fishing
 - Cooking
 - WoodCutting
 - Carpentry
 - Fletching
 - Bushwacking
 - First Aid
 - Enchanting
 
## Lore Development (Expansion 2.0)
- In a Universe known as '(TBD)' in a galaxy called 'Hackulos (STC)'. Our Solar systems sun is knows as the sperk, and our planet is called Devroth. The people of Devroth, From the respectable and wise "High Scolars" to the meek and rarely note worth "Hearthlings"  consider them selves children of The Sperk. A godly 'Singleton' (lets deliberate on a better name for their dieties - some kind of nod to programming knowledge)
- I need to remeber to add sperk as a diety








### CREDTIS
#Music
- Medivel Abient Music
- Stolen Sega Stuff ;)
- Classical song
Phase Shift by Scott Buckley | www.scottbuckley.com.au
Music promoted by https://www.chosic.com/free-music/all/
Creative Commons CC BY 4.0
https://creativecommons.org/licenses/by/4.0/




#Ambience
- LoudLib


#SFX
- 




























***BUG DQ 0302 - 001 START***



```GUI Error: Invalid GUILayout state in DeverQuestWindow view. Verify that all layout Begin/End calls match
UnityEngine.GUIUtility:ProcessEvent (int,intptr,bool&)

ArgumentNullException: Value cannot be null.
Parameter name: target
UnityEngine.Bindings.ThrowHelper.ThrowArgumentNullException (System.Object obj, System.String parameterName) (at <6a469c5cf96a43eab23a293167261e20>:0)
UnityEditor.EditorUtility.SetDirty (UnityEngine.Object target) (at <6dd00658fb454e6fb4c06a416ab8eaa1>:0)
EchoDevGames.DeverQuest.DeverQuestIdentityCatalogService.EnsureRegistry () (at ./Packages/com.echodevgames.deverquest/Editor/DeverQuestIdentityCatalogService.cs:487)
EchoDevGames.DeverQuest.DeverQuestIdentityCatalogService.SetActiveCatalog (EchoDevGames.DeverQuest.DeverQuestIdentityCatalog catalog) (at ./Packages/com.echodevgames.deverquest/Editor/DeverQuestIdentityCatalogService.cs:96)
EchoDevGames.DeverQuest.DeverQuestIdentityCatalogGenerator.GenerateOriginalStarterCatalog () (at ./Packages/com.echodevgames.deverquest/Editor/DeverQuestIdentityCatalogGenerator.cs:256)
EchoDevGames.DeverQuest.DeverQuestWindow.DrawContentScaffolding () (at ./Packages/com.echodevgames.deverquest/Editor/DeverQuestWindow.cs:1305)
EchoDevGames.DeverQuest.DeverQuestWindow.DrawSessionDashboard () (at ./Packages/com.echodevgames.deverquest/Editor/DeverQuestWindow.cs:1200)
EchoDevGames.DeverQuest.DeverQuestWindow.OnGUI () (at ./Packages/com.echodevgames.deverquest/Editor/DeverQuestWindow.cs:339)
UnityEditor.HostView.InvokeOnGUI (UnityEngine.Rect onGUIPosition) (at <6dd00658fb454e6fb4c06a416ab8eaa1>:0)
UnityEditor.DockArea.DrawView (UnityEngine.Rect dockAreaRect) (at <6dd00658fb454e6fb4c06a416ab8eaa1>:0)
UnityEditor.DockArea.OldOnGUI () (at <6dd00658fb454e6fb4c06a416ab8eaa1>:0)
UnityEngine.UIElements.IMGUIContainer.DoOnGUI (UnityEngine.Event evt, UnityEngine.Matrix4x4 parentTransform, UnityEngine.Rect clippingRect, System.Boolean isComputingLayout, UnityEngine.Rect layoutSize, System.Action onGUIHandler, System.Boolean canAffectFocus) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.IMGUIContainer.HandleIMGUIEvent (UnityEngine.Event e, UnityEngine.Matrix4x4 worldTransform, UnityEngine.Rect clippingRect, System.Action onGUIHandler, System.Boolean canAffectFocus) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.IMGUIContainer.HandleIMGUIEvent (UnityEngine.Event e, System.Action onGUIHandler, System.Boolean canAffectFocus) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.IMGUIContainer.HandleIMGUIEvent (UnityEngine.Event e, System.Boolean canAffectFocus) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.IMGUIContainer.SendEventToIMGUIRaw (UnityEngine.UIElements.EventBase evt, System.Boolean canAffectFocus, System.Boolean verifyBounds) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.IMGUIContainer.SendEventToIMGUI (UnityEngine.UIElements.EventBase evt, System.Boolean canAffectFocus, System.Boolean verifyBounds) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.IMGUIContainer.HandleEventBubbleUp (UnityEngine.UIElements.EventBase evt) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.EventDispatchUtilities.HandleEventAcrossPropagationPathWithCompatibilityEvent (UnityEngine.UIElements.EventBase evt, UnityEngine.UIElements.EventBase compatibilityEvt, UnityEngine.UIElements.BaseVisualElementPanel panel, UnityEngine.UIElements.VisualElement target, System.Boolean isCapturingTarget) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer (UnityEngine.UIElements.EventBase evt, UnityEngine.UIElements.BaseVisualElementPanel panel, System.Int32 pointerId, UnityEngine.Vector2 position) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.PointerUpEvent.Dispatch (UnityEngine.UIElements.BaseVisualElementPanel panel) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.EventDispatcher.ProcessEvent (UnityEngine.UIElements.EventBase evt, UnityEngine.UIElements.BaseVisualElementPanel panel) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.EventDispatcher.Dispatch (UnityEngine.UIElements.EventBase evt, UnityEngine.UIElements.BaseVisualElementPanel panel, UnityEngine.UIElements.DispatchMode dispatchMode) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.BaseVisualElementPanel.SendEvent (UnityEngine.UIElements.EventBase e, UnityEngine.UIElements.DispatchMode dispatchMode) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.UIElementsUtility.DoDispatch (UnityEngine.UIElements.BaseVisualElementPanel panel) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.UIElementsUtility.UnityEngine.UIElements.IUIElementsUtility.ProcessEvent (System.Int32 instanceID, System.IntPtr nativeEventPtr, System.Boolean& eventHandled) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.UIEventRegistration.ProcessEvent (System.Int32 instanceID, System.IntPtr nativeEventPtr) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.UIElements.UIEventRegistration+<>c.<.cctor>b__1_2 (System.Int32 i, System.IntPtr ptr) (at <121a28804fa6471c990daf4446fef087>:0)
UnityEngine.GUIUtility.ProcessEvent (System.Int32 instanceID, System.IntPtr nativeEventPtr, System.Boolean& result) (at <8c7a5484f44c4752a0d4812c53f2f7d8>:0)



***BUG DQ 0302 - 001 END***


