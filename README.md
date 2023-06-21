# Projekt im Kurs Labor Games im SS23

Projektname: "Nature's Symphony: Procedurally Generated Fox and Hare Ecosystem"

### Teammitglieder:	

- Alexander Bleisinger (205042)
- Rebecca Biebl (205037)

## Besonderheiten des Projekts / Bedienung:
- Im Branch ai-support-settings, wurden eineige Features entfernt, wodurch es der KI erelichtert werden soll, zu trainieren

## Besondere Leistungen, Herausforderungen und gesammelte Erfahrungen während des Projekts. Was hat die meiste Zeit gekostet?

### KI:
Das Logik bei der Entwicklung einer KI sehr zeitaufwendig ist. Nimmt man das Beispiel der Herdplatte, auf die man nicht fassen soll:
Wir erkennen die Herdplatte, bewegen uns auf sie zu & spüren die Wärme, bis wir uns am Schluss verbrennen
Hier werden mehrere Dinge benötigt, bevor wir überhaupt den logischen Schluss ziehen können: 
Wir müssen...
	- erkennen, dass es eine Herdplatte ist
	- wissen, wie wir uns zu der Herdplatte hinbewegen können
	- die Wärme war nehmen können
	- verwerten das die Herdplatte wehtut
Der letzte Teil ist mit Reward & Penalties relativ einfach zu lösen, die ersten beiden Teile jedoch nicht. Betrachten wir dazu den KI-Hasen:
- Die neue KI (im Vergleich zu Katz & Maus aus dem vorherigen Semester) kann ihren Kopf bewegen.
  Das hat zur Folge, dass die KI erst einmal lernen muss, Kopf und Körper miteinander zu koordinieren, um zu verstehen, ob, wie & in welche Richtung sie sich bewegt
  Daraus folgt für dieses Beispiel: Damit Hasen Karotten essen benötigen sie:
	- Verständnis was eine Karotte ist / müssen sie sehen & erkennen können
	- müssen ihren Kopf & Körper koordinieren, sodass sie sich auf die Karotte zubewegen
	- Das Wissen, wie sie mit der Karotte interagieren können / sollten

Also damit die KIs eine gewisse Logik lernen können, braucht es eine gewisse Grundlage und anschließend sehr viel Zeit, um "Logik" zu erkennen und zu verstehen.
Das Problem zu Beginn des Projektes, war es zu viel Logik ohne direkte Auswirkungen auf die KI gab (keine Reward & Penalties)
Das Leben & Stamina-, sowie Hunger & Durst-System vergab nur Rewards für das lange Überleben, sowie das Sterben.
Dieser Ansatz dürfte nach wie vor funktionieren, der Zeitaufwand, damit die KI diese logischen Prozesse versteht, dürfte aber immens sein.
Ein Beispiel dafür wäre: 
Wenn die Ausdauer einer KI auf 0 sinkt, wird stattdessen, ihre Leben genutzt. 
Wenn die Leben auf 0 sinken, stirbt die KI.
Eine Penalty gab es nur, wenn sie stirbt und eine Belohnung, wenn sie einen weiteren Monat überlebt.	
Mit sehr viel Zeit würde die KI diesen logischen Zusammenhang erkennen, würde aber sehr viel zeit benötigen.
Ein Ansatz, dass die KI dies schneller versteht, wäre eine Penalty Logik wie folgende:	
addReward(-(100-agentHealth)/60 * Time.deltaTime)
Die KI erhält nun umgekehrt linear zu ihren Leben, eine Penalty, wodurch sie deutlich schneller verstehen kann, dass es schlecht ist, wenn ihre Leben sinken.

Dies war der Hauptpunkt, den ich gelernt habe: KIs lernen alles wenn man ihnen genug Zeit und Informationen gibt, jeden auch so komplexen Prozess. 
Ziel sollte aber sein, die KIs durch den Prozess anhand von Rewards & Penalties zu führen, wodurch sie diesen leichter logische Schlüsse ziehen können.

Und selbst, wenn die KI geführt wird, wie in unserem Fall, steigt die Trainingszeit exponentiell an, 
je schwieriger das Problem ist. Deshalb war das eine der zeitaufwendigsten Dinge im Projekt, 
das Training selbst, welches über 12 Stunden dauerte, um im Anschluss auswerten zu können, 
ob das Training erfolgreich war.

## Verwendete Assets, Codefragmente, Inspiration.

	- Katz und Maus - https://github.com/pegasus0112/CatAndMouse-Unity-ml-Agents
	- Quirky Series - Animals Mega Pack Vol 1 - https://assetstore.unity.com/packages/3d/characters/animals/quirky-series-animals-mega-pack-vol-1-137259
	- 

## Video

	- 
