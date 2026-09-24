# ReservationVol — Réservation de vols et cartographie

Application web développée en C# avec ASP.NET Core MVC, permettant
de consulter des vols, d’enregistrer des réservations et de visualiser
les liaisons sur une carte interactive.

Le projet combine la gestion de données métier avec des fonctionnalités
géographiques utilisant PostgreSQL/PostGIS, NetTopologySuite et Leaflet.

> Prototype pédagogique : l’authentification, les contrôles d’accès
> et certains éléments de la configuration géographique restent
> à finaliser. Utiliser uniquement des données fictives pour les essais.

## Fonctionnalités présentes

### Consultation des vols

- Affichage des villes de départ et de destination.
- Consultation des dates de départ et d’arrivée.
- Affichage du prix et du nombre de places indiqué.
- Recherche par ville de départ.
- Filtrage par destination et date de départ.
- Tri des vols par prix, croissant ou décroissant.
- Réinitialisation des filtres.

Les filtres et le tri sont réalisés côté navigateur en JavaScript.

### Réservations

- Formulaire de réservation avec nom et adresse email.
- Association d’une réservation à un vol.
- Enregistrement de la date de réservation.
- Statut initial en attente de confirmation.
- Affichage des réservations et de leur statut.
- Annulation par suppression d’une réservation.

La liste intitulée « Mes réservations » affiche actuellement toutes
les réservations : elle n’est pas encore filtrée par utilisateur.

### Interface de gestion

- Ajout de vols.
- Modification et suppression de vols.
- Consultation des détails d’un vol.
- Consultation des réservations.
- Confirmation d’une réservation.
- Suppression d’une réservation.
- Affichage des réservations confirmées dans le tableau de bord.

Ces interfaces ne sont pas encore protégées par une véritable
authentification et des autorisations.

### Carte des liaisons

- Carte interactive Leaflet avec fond OpenStreetMap.
- Chargement des liaisons depuis un endpoint GeoJSON.
- Représentation des liaisons par des lignes.
- Fenêtres contextuelles avec départ, destination, date et places.
- Ajustement de l’étendue de la carte aux liaisons chargées.

Lors de l’ajout d’un vol, le code tente de créer une géométrie
si les deux villes figurent dans son dictionnaire de coordonnées.

Villes actuellement référencées :

- Casablanca
- Fès
- Rabat
- Marrakech
- Madrid
- Paris
- New York
- Dubai

Les lignes relient des coordonnées de villes. Elles ne représentent
pas des trajectoires aériennes réelles ni un suivi de vols en temps réel.

## Technologies utilisées

| Technologie | Rôle |
|---|---|
| C# / .NET 8 | Langage et plateforme |
| ASP.NET Core MVC | Organisation de l’application web |
| Razor | Génération des vues |
| Entity Framework Core 9 | Accès aux données et migrations |
| Npgsql | Intégration de PostgreSQL |
| PostgreSQL / PostGIS | Données métier et géométries |
| NetTopologySuite | Manipulation des objets géographiques |
| Leaflet 1.9.4 | Carte interactive |
| OpenStreetMap | Fond cartographique |
| GeoJSON | Échange des données géographiques |
| HTML, CSS, Bootstrap et JavaScript | Interface, filtres et tri |

## Organisation du projet

| Chemin | Contenu |
|---|---|
| `VolApp.sln` | Solution Visual Studio |
| `VolApp.csproj` | Configuration et dépendances |
| `Controllers/` | Traitement des requêtes |
| `Models/` | Modèles métier et géographiques |
| `Views/` | Interfaces Razor |
| `Data/` | Contexte Entity Framework Core |
| `Migrations/` | Migrations de base de données |
| `wwwroot/` | Styles, scripts et images |
| `Program.cs` | Configuration et démarrage |

### Contrôleurs principaux

- `ClientController` : consultation des vols et réservations.
- `GestionnaireController` : gestion des vols et des réservations.
- `CarteController` : carte et export GeoJSON.
- `AccountController` : interfaces de connexion de démonstration.
- `HomeController` : pages générales.

## Modèle de données

### Vol

- Identifiant.
- Ville de départ.
- Destination.
- Date et heure de départ.
- Date et heure d’arrivée.
- Prix.
- Nombre de places disponibles.

### Reservation

- Identifiant.
- Nom du client.
- Adresse email.
- Vol associé.
- Date de réservation.
- Indicateur de confirmation.
- Message de statut.

### VolLigne

- Identifiant.
- Vol associé.
- Géométrie de type `LineString`.

Le code de création des lignes utilise le SRID 4326,
avec les coordonnées dans l’ordre longitude, latitude.

## Installation et configuration

### Prérequis

- SDK .NET 8.
- PostgreSQL avec PostGIS disponible.
- Un éditeur compatible avec .NET.
- Un navigateur récent.
- Une connexion Internet pour Leaflet et les tuiles OpenStreetMap.

### 1. Récupérer le projet

```bash
git clone https://github.com/salma-jnt/ReservationVol.git
cd ReservationVol
```

Le dépôt peut également être téléchargé et extrait au format ZIP.

### 2. Restaurer les dépendances

```bash
dotnet restore
```

### 3. Configurer la connexion

Configurer la clé suivante pour votre instance PostgreSQL :

```text
ConnectionStrings:DefaultConnection
```

La chaîne de connexion doit préciser l’hôte, le port, la base,
l’utilisateur et son mot de passe.

Conserver les identifiants sensibles hors du dépôt, par exemple
dans les secrets utilisateur .NET ou une variable d’environnement
nommée `ConnectionStrings__DefaultConnection`.

### 4. Préparer le schéma de base de données

La configuration géographique doit être corrigée avant de considérer
l’installation comme reproductible :

- Les migrations fournies décrivent les vols et les réservations,
  mais ne créent pas l’entité géographique `VolLigne`.
- L’insertion SQL cible `vols_lignes` et la colonne `vol_id`.
- Le modèle Entity Framework ne définit pas de mapping explicite
  correspondant à ces noms.

Il faut harmoniser le mapping et la requête SQL, puis ajouter
une migration pour la table géographique.

Activer également PostGIS dans la base cible, avec un compte
disposant des droits nécessaires :

```sql
CREATE EXTENSION IF NOT EXISTS postgis;
```

Après correction du modèle et création de la migration,
appliquer les migrations avec un outil `dotnet-ef` de version 9
compatible avec le projet :

```bash
dotnet ef database update
```

### 5. Lancer l’application

Une fois la base configurée :

```bash
dotnet run --project VolApp.csproj
```

Ouvrir l’adresse indiquée dans le terminal.

## Routes principales

| Route | Fonction |
|---|---|
| `/Client/Accueil` | Liste des vols et filtres |
| `/Client/Reserver/{id}` | Formulaire de réservation |
| `/Client/MesReservations` | Liste des réservations |
| `/Gestionnaire/Dashboard` | Gestion des vols |
| `/Gestionnaire/ListeReservations` | Gestion des réservations |
| `/Carte/Index` | Carte des liaisons |
| `/Carte/GetVolsGeoJson` | Données géographiques au format GeoJSON |

## Limites actuelles

### Comptes et accès

- La connexion redirige selon le texte de l’adresse email.
- Le mot de passe reçu n’est pas vérifié.
- Aucun compte utilisateur ni session authentifiée n’est créé.
- La page d’inscription ne réalise pas de création de compte.
- Les actions client et gestionnaire ne disposent pas de protection
  effective par rôle.
- Les réservations ne sont pas associées à un utilisateur authentifié.

### Réservations

- Le nombre de places n’est pas décrémenté lors d’une réservation.
- La disponibilité n’est pas contrôlée avant l’enregistrement.
- Aucun paiement, billet électronique ou envoi d’email n’est implémenté.
- L’annulation supprime l’enregistrement au lieu de conserver
  un historique de statut.

### Cartographie

- Le schéma géographique et les migrations doivent être harmonisés.
- Seules les villes du dictionnaire peuvent générer une liaison.
- Une modification du départ ou de la destination ne met pas
  automatiquement à jour la géométrie.
- Le cas d’une carte sans liaison reste à gérer.
- L’enregistrement du vol et de sa géométrie n’est pas atomique :
  le vol peut être enregistré même si la création de la ligne échoue.


## Compétences mobilisées

- Développement web avec ASP.NET Core MVC.
- Modélisation relationnelle avec Entity Framework Core.
- Utilisation de PostgreSQL et de données spatiales PostGIS.
- Manipulation de géométries avec NetTopologySuite.
- Production et affichage de GeoJSON.
- Cartographie interactive avec Leaflet.
- Filtrage et tri de données en JavaScript.

