# ProtoRegistry module

ProtoRegistry is a module that makes adding new content to Dyson Sphere Program easy.

## Usage

Usage of the tool is fairly simple with an example listed below. In order to utilise ProtoRegistry, you must achieve this checklist in order (to register new protodata for items and recipes):
- Create your ResourceData and add it to global list
- Register all localised strings for all new items/recipes you are adding
- Register all items 
- Register all recipes
- etc

(The process is similiar for creating new technologies, models and so on forth)

If you intent on using custom assets, create and import them to a empty unity project. Make sure that path to assets **contains** your **keyword**. Then create asset bundle containing these resources. You can use [this useful tool](https://thunderkit.thunderstore.io/package/CommonAPI/DSPEditorKit/) to do this. 
Additional info on creating custom buildings can be found [here](https://github.com/kremnev8/DSP-Mods/wiki/Creating-prefabs-to-for-machines)

Also it is highly recomended that you use `StartModLoad()` to tell CommonAPI when your mod is adding content. This information is used by CommonAPI later when players remove your mod.

## Example
```csharp
//put this code in your BepInEx plugin class:
public static ResourceData resources;

//Put this code in your awake function

// Let CommonAPI know that your mod is loading
using (ProtoRegistry.StartModLoad(GUID))
{

//Initilize new instance of ResourceData class.
string pluginfolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
resources = new ResourceData(GUID, "<Your Keyword here>", pluginfolder); // Make sure that the keyword you are using is not used by other mod authors.
resources.LoadAssetBundle("<Your bundle Name here>"); // Load asset bundle located near your assembly
resources.ResolveVertaFolder(); // Call this to resolver verta folder. You don't need to call this if you are not using .verta files 
ProtoRegistry.AddResource(resources); // Add your ResourceData to global list of resources

//Creates a custom stringProto for localisation
ProtoRegistry.RegisterString("copperWireDesc", "By extruding copper we can make a component which allows current to be carried"); 
ProtoRegistry.RegisterString("copperWireConc", "You have unlocked production of copper wire. Highly conductive materials are very useful when creating automated devices"); 
copperWireConc

//You can add multilple languages too. Currently English, Chinese and French are supported.
ProtoRegistry.RegisterString("copperWireName", "Copper Wire", "铜线/铜线", "Fil de Cuivre");


//Registers a new item using set parameters and loads it into the game
ItemProto wire = ProtoRegistry.RegisterItem(4500, "copperWireName", "copperWireDesc", "assets/example/copper_wire", 1711);
//Registers a new recipe using set parameters and loads it into the game
RecipeProto recipe = ProtoRegistry.RegisterRecipe(4501, ERecipeType.Assemble, 60, new[] { 1104 }, new[] { 2 }, new[] { wire.ID }, new[] { 1 }, "copperWireDesc"); 

//Registers a new technology using set parameters and loads it into the game
TechProto tech = ProtoRegistry.RegisterTech(1500, "copperWireName", "copperWireDesc", "copperWireConc", "assets/example/copper_wire", new[] {1},
                new[] {1202}, new[] {30}, 1200, new [] {recipe.ID}, new Vector2(9, -3));
}

```

Usage of all functions can be found in [code](https://github.com/kremnev8/CommonAPI/blob/master/CommonAPI/Systems/ProtoRegistrySystem/ProtoRegistry.cs)

## Troubleshooting
I have added assets, but nothing works.
- Make sure you don't try to use ProtoRegistry while loading from something like ScriptEngine. All assets must be registered BEFORE game finishes loading. If you want to reload your code, split you mod into two parts, one that's loaded only once on load, and one being reloaded.
- Make sure that path to assets **contains** your **keyword**. Also make sure to not specity file extension <br>
Valid Path: `assets/customwarp/audio/slowdown`<br>
Invalid path: `assets/audio/slowdown`<br>
Invalid Path: `assets/customwarp/audio/slowdown.mp3`<br>
Invalid Path: `assets/customwarp_slowdown.mp3`. Although this might work, I don't recommend it.<br>
- Make sure you have registerd your asset bundle to ProtoRegistry: ``` ProtoRegistry.AddResource(resources); ```
- Make sure your ID's did not collide with other protos from the same set.

I have added an Item, but it's icon is white and does not display in inventory.
- Make sure your icon has: size of 80x80 pixes, uses RGBA 32 bit, with read/write flag enabled. Here you can see recomended settings
![изображение](https://user-images.githubusercontent.com/12484618/151116615-a8119bc4-5bf3-44d6-aa60-20186e304d27.png)

## Contributing
Pull requests are welcome. Please make sure to test changes before opening pull request.
