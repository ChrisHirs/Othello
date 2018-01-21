        __  ____  _  _  ____  __    __     __  
       /  \(_  _)/ )( \(  __)(  )  (  )   /  \ 
      (  O ) )(  ) __ ( ) _) / (_/\/ (_/\(  O )
       \__/ (__) \_)(_/(____)\____/\____/ \__/

 Deni Gahlinger & Christophe Hirschi - Janvier 2018
====================================================

La fonction d'�valuation de notre intelligence
artificielle se base sur trois concepts.

1. Matrices de pond�ration des cases du plateau
----------------------------------------------------
Il s'agit de donner de l'importance � certaines
cases du plateau de jeu en pla�ant des pond�rations
sur les case de celui-ci. Notre fonction 
d'�valuation poss�de deux tableaux dans lesquelles
elle va extraire les pond�rations en fonction de
l'�tat actuel du jeu. En d�but de partie, le tableau
de pond�ration utilis� favorise les coups jou�s au
centre du plateau et lorsqu'il reste moins de vingt
cases � jouer, l'�valuation prend en compte le
second tableau qui pousse l'IA � jouer les c�t�s
et les bords.

Les bords sont tr�s importants dans les strat�gies
gagnantes de l'Othello, c'est pourquoi nous traitons
les bords de fa�ons sp�cifiques dans notre
�valuation : � tous moments de la partie, les bords
sont privil�gi�s et les cases adjacentes aux bords
sont �vit�es, mais nous tenons compte du fait que si
un bord est d�j� pris, les cases adjacentes ne sont
plus si mauvaises que cela et rapporte quelques
points.

2. Mobilit�
----------------------------------------------------
Il s'agit ici de donner de l'importance aux coups
qui handicape le plus l'adversaire : moins
l'adversaire peut jouer de coups le tour suivant,
plus il est b�n�fique pour nous.

3. Parit�
----------------------------------------------------
La parit� prend en compte le fait que le second
joueur � l'avantage de jouer le dernier tour. Nous
prenons donc acte de ce fait en essayant de tout
faire pour avoir le dernier coup � jouer en for�ant
l'adversaire � passer un coup si celui-ci ne joue
pas le premier.