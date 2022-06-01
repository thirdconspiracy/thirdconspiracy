print('Start #################################################################');

use admin;

db.createUser(
  {
    user: 'root',
    pwd: 'root',
    roles: [
     { role: "root", db: "admin" }
    ]
  }
);

use Catalog;

db.createUser(
  {
    user: 'catalog',
    pwd: 'test123test',
    roles: [
     { role: "readWrite", db: "Catalog" }
    ]
  }
);

print('END #################################################################');
