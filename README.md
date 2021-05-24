# A 2d topdown, base-building/survival game I call Orkhon.

  I started this project to combine everything I learned throughout years. I love modding and was curious about LUA when I started building this that's why I wanted it to be modable so after the core game was done I could just mod it as a hobby. 
  
  I tested Lua with Unity for the first time in this project and it quickly became overwhelming.
Which led me to realise Unity and C# is not really build for it. It can be done but it's just too much of a hussle and effects the performance way too much.
  
  This is also my last project before I moved on to Godot and fell in love.


# Key features:
 - World is procedurally generated.
 - Uses A* for pathfinding. Thanks quill18creates on youtube.
 - Has move, chop, dig, haul and build tasks already implemented.
 - Objects, items and tasks can be modded using LUA. Check "Assets/StreamingAssets/Mods/Vanilla/Objects/Stone/Scripts/scripts.lua".

# What's missing (other than content):
 - There is no optimization for path finding. Check https://www.youtube.com/watch?v=RMBQn_sg7DA for an aproach. http://www.shiningrocksoftware.com/2013-11-21-more-bugs-pathfinding-problems/ is also worth reading.
