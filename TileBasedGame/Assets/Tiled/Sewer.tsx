<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.10" tiledversion="1.10.2" name="Sewer" tilewidth="16" tileheight="16" tilecount="352" columns="16" objectalignment="topleft">
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
  <properties>
   <property name="TestScript.value" type="int" value="69"/>
   <property name="components" value="TileBasedGame.TestScript,TileBasedGame.TestScript2"/>
  </properties>
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
