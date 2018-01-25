# Othello
Implémentation du jeu Othello créée dans le cadre du cours de .NET et d’algorithme génétique.

## Évaluation
La fonction d'évaluation de notre intelligence
artificielle se base sur trois concepts.

### 1. Matrices de pondération des cases du plateau
Il s'agit de donner de l'importance à certaines cases du plateau de jeu en plaçant des pondérations sur les case de celui-ci. Notre fonction  d'évaluation possède deux tableaux dans lesquels elle va extraire les pondérations en fonction de l'état actuel du jeu. En début de partie, le tableau de pondération utilisé favorise les coups joués au centre du plateau et lorsqu'il reste moins de vingt cases à jouer, l'évaluation prend en compte le second tableau qui pousse l'IA à jouer les côtés et les bords.

Les bords sont très importants dans les stratégies
gagnantes de l'Othello, c'est pourquoi nous traitons les bords de façons spécifiques dans notre évaluation : à tous moments de la partie, les bords sont privilégiés et les cases adjacentes aux bords sont évitées, mais nous tenons compte du fait que si un bord est déjà pris, les cases adjacentes ne sont plus si mauvaises que cela et rapporte quelques points.

### 2. Mobilité
Il s'agit ici de donner de l'importance aux coups qui handicape le plus l'adversaire : moins l'adversaire peut jouer de coups le tour suivant, plus il est bénéfique pour nous.

### 3. Parité
La parité prend en compte le fait que le second joueur à l'avantage de jouer le dernier tour. Nous prenons donc acte de ce fait en essayant de tout faire pour avoir le dernier coup à jouer en forçant l'adversaire à passer un coup si celui-ci ne joue pas le premier.
