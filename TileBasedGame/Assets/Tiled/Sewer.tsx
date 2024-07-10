<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.10" tiledversion="1.11.0" name="Sewer" tilewidth="16" tileheight="16" tilecount="352" columns="16" objectalignment="topleft">
 <image source="../Textures/tilesets/sewer_1.png" width="256" height="352"/>
 <tile id="0" type="Obstacle">
  <properties>
   <property name="components" type="object" value="0"/>
  </properties>
  <objectgroup draworder="index" id="3">
   <object id="3" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="1" type="Obstacle">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="2" type="Obstacle">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="3" type="Obstacle"/>
 <tile id="4" type="Obstacle"/>
 <tile id="5">
  <objectgroup draworder="index" id="3">
   <object id="2" x="4" y="16">
    <polygon points="0,0 12,0.125 12,-11"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="7">
  <properties>
   <property name="components" value="TileBasedGame.Goal"/>
  </properties>
 </tile>
 <tile id="16" type="Obstacle">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="17" type="Obstacle">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="18" type="Obstacle">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="19" type="Obstacle"/>
 <tile id="20" type="Obstacle"/>
 <tile id="23">
  <properties>
   <property name="components" value="TileBasedGame.Goal"/>
  </properties>
 </tile>
 <tile id="32" type="Obstacle">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="33" type="Obstacle">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="34" type="Obstacle">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="35" type="Obstacle"/>
 <tile id="36" type="Obstacle">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="48" type="Obstacle"/>
 <tile id="49" type="Obstacle"/>
 <tile id="50" type="Obstacle">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="55" type="Obstacle"/>
 <tile id="56" type="Obstacle"/>
 <tile id="57" type="Obstacle"/>
 <tile id="58" type="Obstacle"/>
 <tile id="64" type="Obstacle"/>
 <tile id="65" type="Obstacle"/>
 <tile id="66" type="Obstacle"/>
 <tile id="67">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.ThinPlatform"/>
  </properties>
 </tile>
 <tile id="68">
  <animation>
   <frame tileid="68" duration="200"/>
   <frame tileid="69" duration="200"/>
   <frame tileid="70" duration="200"/>
  </animation>
 </tile>
 <tile id="71" type="Obstacle"/>
 <tile id="72" type="Obstacle"/>
 <tile id="73" type="Obstacle"/>
 <tile id="74" type="Obstacle"/>
 <tile id="82">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="84" type="Obstacle"/>
 <tile id="85" type="Obstacle"/>
 <tile id="86" type="Obstacle"/>
 <tile id="187">
  <animation>
   <frame tileid="187" duration="300"/>
   <frame tileid="188" duration="300"/>
   <frame tileid="189" duration="300"/>
   <frame tileid="190" duration="300"/>
  </animation>
 </tile>
 <tile id="201">
  <animation>
   <frame tileid="201" duration="500"/>
   <frame tileid="202" duration="200"/>
   <frame tileid="203" duration="200"/>
   <frame tileid="204" duration="200"/>
   <frame tileid="205" duration="200"/>
   <frame tileid="206" duration="200"/>
  </animation>
 </tile>
 <tile id="214">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="215">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="216">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="217">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="220">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="221">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="222">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="223">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="226">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="228">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="229">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="230">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="231">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="233">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="234">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="235">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="244">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="245">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="246">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="247">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="248">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="249">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="250">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="251">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="264">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="265">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="268">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="269">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="270">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="271">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="276">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="277">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="292">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
  <animation>
   <frame tileid="292" duration="200"/>
   <frame tileid="293" duration="200"/>
   <frame tileid="294" duration="200"/>
   <frame tileid="295" duration="200"/>
   <frame tileid="296" duration="200"/>
   <frame tileid="297" duration="200"/>
  </animation>
 </tile>
 <tile id="293">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="294">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="295">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="296">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="297">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="298">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="299">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="324">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <tile id="325">
  <properties>
   <property name="components" value="SDL2Engine.Tiled.DamageTile"/>
  </properties>
 </tile>
 <wangsets>
  <wangset name="Stone1" type="mixed" tile="-1">
   <wangcolor name="" color="#ff0000" tile="-1" probability="1"/>
   <wangtile tileid="0" wangid="1,0,0,0,0,0,1,1"/>
   <wangtile tileid="2" wangid="1,1,1,0,0,0,0,0"/>
   <wangtile tileid="32" wangid="0,0,0,0,1,1,1,0"/>
   <wangtile tileid="34" wangid="0,0,1,1,1,0,0,0"/>
  </wangset>
 </wangsets>
</tileset>
