# Learning
This repository serves as a base for a 2D Godot game, specifically one that involves more complex movement, i.e., movement that has many factors that affect it in various ways. This complex movement can be done while maintaining modularity and reusability between these different factors of movement. This is because of the "velocity source/aggregate" and "modifiers/value with modifiers" software design used (more info in the [software design notes](#software-design-notes)). The movement also utilizes the power of Godot's Tweens for smoother and more customizable movement.

The current code in this repository provides an example usage of this software design, in the form of a 2D platformer MVP. Though, the design can be used for non-platformer games, and the design can be used for 3D games (though, this project doesn't support 3D complex movement).

The player in the example game MVP can be controlled with the following keyboard keys:
- A or Left, to move left
- D or Right, to move right
- W, Up, or Space, to jump

## Environment
This project uses Godot 4.2 ([4.2.1-stable with .NET](https://godotengine.org/download/archive/4.2.1-stable/) was used; .NET is needed since this project uses C# instead of GDScript). .NET 6.0 was used for this project.

## How to use (developer)
### Complex Movement
Here is how to create complex movement for an entity in Godot using this repository:
1. In the Godot editor, create a `CharacterBody2D` node, and create and attach a script that extends `VelocityAggregatingKinematicComp` (located in `Scripts/Entity/Physics/`). For this example, we'll call the class `Player`.
2. Create a child Node for that `CharacterBody2D`, and create and attach a script that extends `VelocityAggregate` (located in `Scripts/Entity/Physics/`). For this example, we'll call the class `PlayerVelocity` (you can also see `Scripts/Entity/Physics/PlayerVelocityAggregate.cs` to see an example of how this would be coded).
3. In the `CharacterBody2D` node, select the `PlayerVelocity` node as the value for the field "Velocity Replacer".
4. Come up with the velocity sources (and possibly any "intermediates") for your `PlayerVelocity`. For this, you'll need to think about how you want your player to be able to move, and break that movement down into different, independent components.
   - For example, the velocity of my player in this repository breaks down into two components: `Falling` (velocity affected by gravity) and `LeftRight` (velocity due to the player wanting to move left or right) (located in `Scripts/Entity/Physics/VelocitySources/`.
   - Note that some desired behaviors are not independent of these components. For example, my player can also jump and drag on walls, but these are things that are affected by gravity. So, instead of creating a velocity source for these behaviors, you would want to create an "intermediate" (they don't have a common superclass/interface) that calls the methods of the relevant velocity source(s) to affect velocity. For example, the intermediates `Jumping` and `WallDragging` (located in `Scripts/Entity/Physics/Intermediate/`) call upon `Falling`'s methods to affect falling velocity.
5. Create your velocity sources and intermediates. For each of the ones you came up with:
   1. Create a child Node for your `PlayerVelocity` node, and create and attach a script that extends `VelocitySource` (if it's an independent component) or `Node` (if it's an intermediate).
   2. Program the necessary behavior for the velocity source/intermediate. For velocity sources, be sure to read the documentation comments in `VelocitySource` to see what it provides. Note that the velocity sources shouldn't know about or call any of the intermediates.
      - When programming your intermediates to modify your velocity sources, you might want to have your intermediates give modifiers to values in the velocity source. See [Data Modifiers](#data-modifiers) for how to give and accept modifiers.
   3. Create a child Node for your `PlayerVelocity` node, and create and extend a script that extends `DefaultPhys` (located in `Scripts/Entity/Physics/`). When programming this script, you take `KinematicComp` physics interactions and call methods in your velocity source/intermediate as appropriate, by overriding the appropriate "On..." methods in `DefaultPhys`.
      - Do this instead of having your velocity source/intermediate store and use a reference to the `KinematicComp`/`Player`/etc. This is so that you're able to specify different physics behavior if needed (e.g., by subclassing your script, or by unsubscribing from the events that call specific "On..." methods), instead of being stuck with one set of physics behaviors. This can be useful when reusing your velocity sources/intermediates for a different entity, and want to specify (slightly) different physics behaviors for it.
6. Program your `PlayerVelocity` to define methods for your `Player` to call, to affect the velocity sources and intermediates (for example, my `PlayerVelocityAggregate` has public methods `MoveLeft`, `AttemptJump`, etc.). Your `PlayerVelocity` also needs to define any interactions and rules among intermediates and any (non-default) physics interactions, so that your `PlayerVelocity` behaves the way you want it to.
7. When making other entities, you can now reuse the velocity sources, intermediates, and default physics you made for your `Player`!

### Data Modifiers
When designing an "intermediate" (see above for more info on intermediates) to affect a velocity source, you might want to modify the data that the velocity source uses for its movement. For example, my `Falling` velocity source uses a `FallingData` (located in `Scripts/Entity/Physics/VelocitySources/`) resource to determine falling velocity, acceleration, etc. Then, my `WallDragging` affects my `Falling` to simulate the vertical movement of dragging on a wall. It does this by adding modifiers to the `FallingData`'s values to make the `Falling`'s movement feel more like wall dragging than actual falling.

Let's first walk through modifiers. Two core interfaces are at play with modifiers: `IModifier` and `IValueWithModifiers`. `IModifier` calculates an (arbitrary) value based on a given value, and `IValueWithModifiers` can aggregate zero to many `IModifiers` by calling them one-by-one on base/modified values to get a single final modified value. More information is in the documentation comments of these interfaces, located in `Scripts/Values/Modifiers`.

To utilize data modifiers for your velocity sources and intermediates, you first need to have your data values be able to accept modifiers. You can implement your own classes implementing `IValueWithModifiers`, or use `ValueWithModifiers` (a basic implementation).

But, you will probably want to use/extend `ResourceWithModifiers` (located in `Scripts/Values/Groups`), which holds a group of `IValueWithModifier`s (see also `IValueWithModifiersGroup`, located in `Scripts/Values/Groups/`). This is because `ResourceWithValues` also extends Godot's `Resource`, so you can specify the base values for these `IValueWithModifier`s in the Godot editor, as well as save these base values to be reused elsewhere. `FallingData` is an example of how a `ResourceWithModifiers` would be implemented.

Second, you need to make the modifier(s) for the data. You can create your own modifiers by implementing `IModifier`, but there are already several basic implementations in `Scripts/Values/Modifiers`.

But, you will probably want to use/extend `ModifierResource` (located in `Scripts/Values/Groups`), which holds a group of `IModifier`s (see also `IModifierGroup`, located in `Scripts/Values/Groups/`) that modify specific `IValueWithModifier`s in an `IValueWithModifiersGroup`. `FallingDataMultiplier` is an example of how a `ModifierResource` would be implemented. (however, `WallDragging` uses `WallDraggingData` to modify a `Falling`, and `WallDraggingData` is both a `ResourceWithModifiers` and an `IModifierGroup` (since the wall dragging data itself can be modified)).

## Software Design Notes
to do

## Acknowledgement
This project started off as a learning project for me to learn Godot (hence the repository name "Learning") that followed [this tutorial](https://www.youtube.com/playlist?list=PL9FzW-m48fn0i9GYBoTY-SI3yOBZjH1kJ), but has since then deviated greatly in terms of code, content, and goals (and the tutorial used GDScript and not C#).
