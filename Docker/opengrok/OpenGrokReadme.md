# OpenGrok Docker

## Setup

Create the following directories
```
C:\dev\docker\opengrok
C:\dev\docker\opengrok\data
C:\dev\docker\opengrok\etc
```

Create your Compose file (new text document)
```YAML
version: "3"

# More info at https://github.com/oracle/opengrok/docker/
services:
  opengrok:
    container_name: opengrok
    image: opengrok/docker:latest
    ports:
      - "8080:8080/tcp"
    environment:
      REINDEX: '60'
    # Volumes store your data between container upgrades
    volumes:
       - 'C:\\work:/opengrok/src/'  # source code
       - 'C:\\dev\docker\opengrok\etc:/opengrok/etc/'  # folder contains configuration.xml
       - 'C:\\dev\docker\opengrok\data:/opengrok/data/'  # index and other things for source code
```

### In Powershell (Administrator)

Pull and bring up image
```bat
docker pull opengrok/docker
cd \dev\docker\opengrok
docker-compose up -d
```

## Verification

### Verify Image is running
```bat
docker ps
docker exec -it opengrok bash
cd /opengrok/src
ls
```

### Verify Page loads

Navigate to http://localhost:8080/


