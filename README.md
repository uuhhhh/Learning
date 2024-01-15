# Learning

This repository serves as a base for a 2D Godot game, specifically one that involves more complex
movement, i.e., movement that has many factors that affect it in various ways. This complex movement
can be done while maintaining modularity and reusability between these different factors of
movement. This is because of the "velocity source/aggregate" and "modifiers/value with modifiers"
software design used (more info in the [software design notes](#software-design-notes)). The
movement also utilizes the power of Godot's Tweens for smoother and more customizable movement.

The current code in this repository provides an example usage of this software design, in the form
of a 2D platformer MVP. Though, the design can be used for non-platformer games, and the design can
be used for 3D games (though, this project doesn't support 3D complex movement).

The player in the example game MVP can be controlled with the following keyboard keys:

- A or Left, to move left
- D or Right, to move right
- W, Up, or Space, to jump

Other features of this 2D platformer MVP:

- Wall jumping and double jumping (the double jump resets when touching the ground or a wall)
- Wall dragging, which happens when the bottom half of the player is touching a wall, and the
  left/right input is being inputted towards the wall (but once the player is wall dragging, the
  left/right input can be let go and the player will still wall drag; this behavior can be changed
  in Velocity Aggregate code)
- "Wall snapping", where if the player inputs toward the wall a short time after falling from a
  ledge, they will snap towards the wall; this is to make wall dragging off a ledge easier
- Particles that emit when walking, jumping, and landing from a large enough height
- Fine-grained configurability of the player's movement using Godot's Resources

## Repository Layout

- `resources/`: Godot Resource data used by the platformer MVP
- `scenes/`: Godot Scenes for the platformer MVP
- `Scripts/`: All of the code for this game base
    - `Effects/`: Code concerning the particle effects emitted
    - `Entity/`: Code for the main components that entities (currently the only entity is the
      player) are composed of
        - `Physics/`: Code for the movement of entities; also
          contains [Velocity Aggregate](#velocity-sourcesaggregate-design) objects
            - `Intermediate/`: Code for "Intermediates", components that perform common operations
              for Velocity Sources
            - `VelocitySources/`: Code for [Velocity Source](#velocity-sourcesaggregate-design)
              objects, the individual components that make up an entity's movement
    - `Environment/`: Code concerning with associating data and modifiers with objects in an
      entity's environment (most code here is currently unused)
    - `Values/`: Code concerning [Data Modifiers](#data-modifiers)
        - `Groups/`: Code for objects that group modifiers together or group values with modifiers
          together
        - `Modifiers/`: Code for objects that apply modifiers to a value, or hold a value and its
          modifiers
- `textures/`: Image assets used by the platformer MVP
- `uml/`: UML diagrams for the software designs for this project

## Environment

This project uses Godot
4.2 ([4.2.1-stable with .NET](https://godotengine.org/download/archive/4.2.1-stable/) was used; .NET
is needed since this project uses C# instead of GDScript). .NET 6.0 was used for this project.

## How to use (developer)

To use this project as a developer:

1. Download the version of Godot and install the version of .NET listed
   in [Environment](#environment).
2. Clone this repository to your local machine.
3. Open Godot, and import the root directory of the cloned repository. From here, you can start
   editing the project. (NOTE: when performing these steps, you might get an error running the
   project for the first time, but not for subsequent runs)

### Complex Movement

Here is how to create complex movement for an entity in Godot using this repository:

1. In the Godot editor, create a `CharacterBody2D` node, and create and attach a script that
   extends `VelocityAggregatingKinematicComp` (located in `Scripts/Entity/Physics/`). For this
   example, we'll call the class `Player`.
2. Create a child Node for that `CharacterBody2D`, and create and attach a script that
   extends `VelocityAggregate` (located in `Scripts/Entity/Physics/`). For this example, we'll call
   the class `PlayerVelocity` (you can also see `Scripts/Entity/Physics/PlayerVelocityAggregate.cs`
   to see an example of how this would be coded).
3. In the `CharacterBody2D` node, select the `PlayerVelocity` node as the value for the field "
   Velocity Replacer".
4. Come up with the velocity sources (and possibly any "intermediates") for your `PlayerVelocity`.
   For this, you'll need to think about how you want your player to be able to move, and break that
   movement down into different, independent components.
    - See ["Velocity Sources/Aggregate" Design](#velocity-sourcesaggregate-design) for more
      information on the design of velocity sources and intermediates.
5. Create your velocity sources and intermediates. For each of the ones you came up with:
    1. Create a child Node for your `PlayerVelocity` node, and create and attach a script that
       extends `VelocitySource` (if it's an independent component) or `Node` (if it's an
       intermediate).
    2. Program the necessary behavior for the velocity source/intermediate. For velocity sources, be
       sure to read the documentation comments in `VelocitySource` to see what it provides. Note
       that the velocity sources shouldn't know about or call any of the intermediates.
        - When programming your intermediates to modify your velocity sources, you might want to
          have your intermediates give modifiers to values in the velocity source.
          See [Data Modifiers](#data-modifiers) for how to give and accept modifiers.
    3. Create a child Node for your `PlayerVelocity` node, and create and extend a script that
       extends `DefaultPhys` (located in `Scripts/Entity/Physics/`). When programming this script,
       you take `KinematicComp` physics interactions and call methods in your velocity
       source/intermediate as appropriate, by overriding the appropriate "On..." methods
       in `DefaultPhys`.
        - Do this instead of having your velocity source/intermediate store and use a reference to
          the `KinematicComp`/`Player`/etc. This is so that you're able to specify different physics
          behavior if needed (e.g., by subclassing your script, or by unsubscribing from the events
          that call specific "On..." methods), instead of being stuck with one set of physics
          behaviors. This can be useful when reusing your velocity sources/intermediates for a
          different entity, and want to specify (slightly) different physics behaviors for it.
6. Program your `PlayerVelocity` to define methods for your `Player` to call, to affect the velocity
   sources and intermediates (for example, my `PlayerVelocityAggregate` has public
   methods `MoveLeft`, `AttemptJump`, etc.). Your `PlayerVelocity` also needs to define any other
   interactions and rules among your velocity sources, intermediates, and any (non-default) physics
   interactions, so that your `PlayerVelocity` behaves the way you want it to.
7. When making other entities, you can now reuse the velocity sources, intermediates, and default
   physics you made for your `Player`!

### Data Modifiers

When designing an "intermediate" (see above for more info on intermediates) to affect a velocity
source, you might want to modify the data that the velocity source uses for its movement. For
example, my `Falling` velocity source uses a `FallingData` (located
in `Scripts/Entity/Physics/VelocitySources/`) resource to determine falling velocity, acceleration,
etc. Then, my `WallDragging` affects my `Falling` to simulate the vertical movement of dragging on a
wall. It does this by adding modifiers to the `FallingData`'s values to make the `Falling`'s
movement feel more like wall dragging than actual falling.

Let's first walk through modifiers. Two core interfaces are at play with modifiers: `IModifier`
and `IValueWithModifiers`. `IModifier` calculates an (arbitrary) value based on a given value,
and `IValueWithModifiers` can aggregate any number of `IModifiers` by calling them one-by-one on
base/modified values to get a single final modified value. More information is in the documentation
comments of these interfaces, located in `Scripts/Values/Modifiers`.

To utilize data modifiers for your velocity sources and intermediates, you first need to have your
data values be able to accept modifiers. You can implement your own classes
implementing `IValueWithModifiers`, or use `ValueWithModifiers` (a basic implementation).

But, you will probably want to use/extend `ResourceWithModifiers` (located
in `Scripts/Values/Groups`), which holds a group of `IValueWithModifier`s (see
also `IValueWithModifiersGroup`, located in `Scripts/Values/Groups/`). This is
because `ResourceWithValues` also extends Godot's `Resource`, so you can specify the base values for
these `IValueWithModifier`s in the Godot editor, as well as save these base values to be reused
elsewhere. `FallingData` is an example of how a `ResourceWithModifiers` would be implemented.

Second, you need to make the modifier(s) for the data. You can create your own modifiers by
implementing `IModifier`, but there are already several basic implementations
in `Scripts/Values/Modifiers`.

But, you will probably want to use/extend `ModifierResource` (located in `Scripts/Values/Groups`),
which holds a group of `IModifier`s (see also `IModifierGroup`, located in `Scripts/Values/Groups/`)
that modify specific `IValueWithModifier`s in an `IValueWithModifiersGroup`. `FallingDataMultiplier`
is an example of how a `ModifierResource` would be implemented. (however, `WallDragging`
uses `WallDraggingData` to modify a `Falling`, and `WallDraggingData` is both
a `ResourceWithModifiers` and an `IModifierGroup` (since the wall dragging data itself can be
modified)).

## Software Design Notes

### "Velocity Sources/Aggregate" Design

In order to perform complex movement while maintaining modularity and reusability, the different
causes of movement are split into different, independent components called "velocity sources". A "
velocity aggregate" is composed of these velocity sources, and can produce a single velocity vector
by taking an aggregate of the velocity vectors for all the velocity sources (by default, this is
just the sum of all the velocity vectors). This single velocity vector would represent the velocity
of the player or some other entity.

For example, the velocity of my player in this repository breaks down into two
components: `Falling` (velocity affected by gravity) and `LeftRight` (velocity due to the player
wanting to move left or right) (located in `Scripts/Entity/Physics/VelocitySources/`.

Note that when creating the velocity sources for an entity's movement, some desired behaviors are
not independent of these components. For example, my player can also jump and drag on walls, but
these are both things that are affected by gravity, something already covered by `Falling`. So,
instead of creating a velocity source for these behaviors, you would want to create an
"intermediate" (they currently don't have a common superclass/interface) that calls the methods of
the relevant velocity source(s) to affect velocity. For example, the intermediates `Jumping`
and `WallDragging` (located in `Scripts/Entity/Physics/Intermediate/`) call upon `Falling`'s methods
to affect falling velocity.

The velocity sources themselves are independent of each other and don't know about each other, but
the velocity aggregate may produce rules and behaviors for interactions (one-way or two-way) between
velocity sources/intermediates, unbeknownst to the velocity sources and intermediates. The velocity
aggregate is where all of these velocity sources and intermediates come together. So, when
subclassing a velocity source, the job of the subclass is to make sure that the specific velocity
sources and intermediates concerned by the subclass act nicely with each other, to produce the
desired behavior. This is done here so that it doesn't have to be done in the code for the velocity
sources and intermediates (which would've introduced unwanted dependencies between certain velocity
sources/intermediates).

This design uses the Component pattern, where the velocity sources and intermediates are the
components, and the velocity aggregates compose these components.

#### Motivation

In [a much earlier commit](https://github.com/uuhhhh/Learning/tree/43573f5a37a382273acd1792acb342684beda854)
in this repository, the code for the player movement was in one class, including left/right
movement, ground/air/wall jumping, coyote jumping, jump buffering, jump cancelling, wall dragging,
and smooth movement using Tweens. It did work properly, and it took significantly less time to code.
And for some project scopes where movement isn't complex, it'd be sufficient to keep player movement
code in one class. However, there were key drawbacks to that approach, which this project aims to
solve.

First, movement behaviors weren't reusable between different entities. While currently the only
entity is the player, if one wanted to make another entity with some of the player's movement
behaviors and some of its own movement behaviors, nontrivial amounts of code would have to be
duplicated. This is unsustainable, especially if there were to be even more different entities. The
separation of movement behaviors into independent, reusable components aims to solve this issue,
where the entities can be composed of the components of the movement features they need, without
duplicating code.

Second, the more different movement features are added to this player movement class, the less
maintainable the class becomes. This is because with more movement features, there are more and more
rules for these features and their interactions. Even with just the 2D platformer MVP, there are a
lot of rules in play, even if it may not look like it. Some examples are:

- What happens when the player transitions from the air to the ground while stopping, and there's
  different horizontal movement acceleration for the ground and air
- How much control the player should have over their horizontal movement speed moments after a wall
  jump
- The exact conditions for when the player should start wall dragging instead of falling normally,
  and when they should stop wall dragging
- What should happen if the player touches the ground or a wall while a coyote jump timer is still
  active
- And much more

With more and more rules, it can become harder to make changes to this player movement class without
breaking something, especially when trying to utilize the power of Godot's Tweens while doing so.
So, the separation of movement behaviors into components accomplishes another thing: it separates
the concerns of different rules into different classes. For instance, `Falling` can concern rules of
when exactly to accelerate and decelerate; `Jumping` can concern rules of which type of jump (
ground/air/wall jump) to use and when, as well as rules of coyote jumping and jump buffering;
velocity sources and intermediates, internally, don't have to worry about Tweens from other velocity
sources; etc. `PlayerVelocityAggregate` no longer has to worry about lower-level rules such as exact
Tween creation processes and the internal rules of its velocity sources and intermediates. The
velocity sources and intermediates form an API that abstracts those details away, so
`PlayerVelocityAggregate` can just worry about higher-level rules.

#### UML Diagram

The `uml` directory of this repository contains two UML diagrams that outline this
design. `uml/velocity-sources/velocity-sources.drawio.html` has a class diagram that summarizes some
of the classes that this design concerns, as well as the dependencies between these
classes. `uml/velocity-simplified/velocity-simplified.drawio.html` has a much-simplified diagram
that visually outlines the dependencies between different parts of this design. These diagrams were
made using [draw.io](https://app.diagrams.net/).

### "Data Modifiers" Design

In order to modify a data value (for example, how fast the player should move horizontally when on
the ground), when there may be other actors that want to modify this same data value, we would like
to maintain independence (as much of it as possible) from these other actors, in terms of how we add
our modification to this data value.

The way that this project does this is by having the data value (`IValueWithModifiers`) hold any
number of modifiers (`IModifier`). Each `IModifier` can take in a value and output a modified value
based on that value. For the `IValueWithModifiers` to calculate the modified value, the first
modifier gets applied to the unmodified value, then the second modifier gets applied to the first
modifier's calculated value, and so on. The order the `IModifier`s are applied are based on the
priority property for each of them. The only spot where these `IModifier`s have dependence on each
other is with the priority property, and that is mitigated by having this property be an enum (
see `Scripts/Values/Modifiers/ModifierPriority.cs`). They are independent everywhere else.

These `IModifier`s act in a way like components of an `IValueWithModifiers`. Unlike with the
velocity sources, though, the `IValueWithModifiers` and its implementers don't work with
specific `IModifiers`: they just apply the `IModifier`s they're given, without knowing their
implementation or how they modify given values. This has the added benefit of ensuring independence
of `IValueWithModifiers` from specific `IModifier`s.

#### Motivation

When I was first implementing the intermediate `WallDragging`, I needed it to modify `Falling` so
that the falling movement was instead more like the movement of dragging down a wall. Originally, I
did this by having the `WallDragging` swap out the `FallingData` that the `Falling` used for
determining fall movement, for a different `FallingData`. However, this solution had a problem: if I
were to make another intermediate that modified a `Falling` in the same way, there would be a
conflict between the two. What happens when `WallDragging` does its swap, the other one does its
swap, and `WallDragging` swaps it back? What if I wanted the effects of the two to somehow stack?

This is the problem that the data modifiers design aims to solve. By having the values
in `FallingData` accept any amount of modifiers, `WallDragging` and any other intermediates can add
and remove their modifiers to and from `FallingData` independently of each other, and the effects of
the different modifiers can stack without the different intermediates having to know about it.

## Acknowledgement

This project started off as a learning project for me to learn Godot (hence the repository name "
Learning") that
followed [this tutorial](https://www.youtube.com/playlist?list=PL9FzW-m48fn0i9GYBoTY-SI3yOBZjH1kJ),
but has since then deviated greatly in terms of code, content, and goals (also the tutorial used
GDScript and not C#).
