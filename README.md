# Lights On Deployment
As you should know from our report, we heavily relied upon
CI/CD to automate and ensure a consistent build envrionment.
As a result, we will only give instructions on how to build
and deploy our system using CI/CD.

## Requirements
- GitHub account
- GitHub actions
- Unity Free Account
- Ubuntu Server 20.04.2

## Setting up GitHub
We had several different repositories for different aspects of our project
- `Lights-on-game` for the Unity Project
- `Lights-on-assets` for editor specific asset files (e.g. `.blend` or `.psd`)
- `Lights-on-website` for website content and JavaScript modules
- `Lights-on-signaling` for the WebRTC signaling server

Only `Lights-on-game` and `Lights-on-website` had CI/CD as the `Lights-on-signaling`
server was deployed only once and the assets reposiory did not need CI/CD.

### Adding reposiory secrets
The following secrets need to be added to the `Lights-on-game` and `Lights-on-website` repositories.

| Key           | Example value                                                          | Description
| :--           | :------------                                                          | :----------                                  |
| HOST          | `lights-on.icedcoffee.dev`                                             | The IP or Domain of the server               |
| PORT          | `22`                                                                   | The SSH port of the server                   |
| UNAME         | `lightson`                                                             | The username to log into the server          |
| KEY           |                                                                        | The SSH *private* key to log into the server   |
| WEBHOOK_ID    | `826577979843805234`                                                   | The Discord Webhook ID                       |
| WEBHOOK_TOKEN | `qKSb4LqC6MvGXDlXnNfAzZvIZujm5XARVbpWOVCJjIZeR4WmLP_cjHy37BHvx2I7zoza` | Discord token                                |

Please feel free to use the provided Discord Webhook credentials
if you do not wish to set up your own Discord server.

### Acquiring a Unity Licence
On top of this, the `Lights-on-game` requires an additional secret: `UNITY_LICENSE`
To get this you must follow the steps outlined in the [Game CI docs](https://game.ci/docs/github/activation)

## Setting up the Server
### The user
An application specific user was set up for this project.
Their home directory was set to be `/var/www/lights-on.icedcoffee.dev/` which is where
`nginx` servers files for the domain from.

This means that deploying the built items from CI was easier as the `ssh` connection
drops the user into the correct directory from the outset.

For security purposes, this user could only access files in this directory.

### Nginx
`nginx` was used to serve the website files and as a reverse proxy for the signaling server.
Nginx can be installed via the `apt` package manager using ```sudo apt install nginx```

The site is the configured via the `/etc/nginx/sites-available/lights-on.icedcoffee.dev` files
which reads as follows:
```
server {
  root /var/www/lights-on.icedcoffee.dev;

  index index.html index.htm index.nginx-debian.html;

  server_name lights-on.icedcoffee.dev;

  gzip_static always;
  gzip_proxied expired no-cache no-store private auth;
  gunzip on;

  error_page 404 = /404.html;
  location = /404.html {
    internal;
  }

  location /voice/ {
    proxy_set_header   Host                 $host;
    proxy_set_header   X-Real-IP            $remote_addr;
    proxy_set_header   X-Forwarded-For      $proxy_add_x_forwarded_for;
    proxy_set_header   X-Forwarded-Proto    $scheme;

    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection "upgrade";
    proxy_pass http://localhost:8080/ws/;
  }

  location / {
    try_files $uri $uri/ =404;
  }
}
```

[certbot](https://certbot.eff.org) was then used to automatically configure
the SSL certificates and forwarding of `http` to `https`.

certbot is installed via apt with `sudo apt install certbot`
then run cerbot using `sudo certbot --nginx`. When prompted to, select
the correct domain, in this case `lights-on.icedcoffee.dev`, and then
select to automatically configure https forwarding.

It is interesting to note that the `/voice/` directory
in the configuration is set to reverse proxy the connection to the
signaling server. This is done via a WebSocket connection therefore
appropritate headers are set to enable this type of communication.

Then start nginx via systemd: `systemctl start nginx`

### Coturn
A coturn server is needed to enable the WebRTC communication.

This is installed via apt using `sudo apt install coturn`

Coturn is configured via the `/etc/turnserver.conf` file

The following configuration was used:
```
fingerprint
lt-cred-mech
server-name=lights-on.icedcoffee.dev
user=lights:shawn
realm=lights-on.icedcoffee.dev
user-quota=100
stale-nonce=600
cert=/etc/letsencrypt/live/lights-on.icedcoffee.dev/cert.pem
pkey=/etc/letsencrypt/live/lights-on.icedcoffee.dev/privkey.pem
proc-user=turnserver
proc-group=turnserver
```

This then needs to be started via systemd using `systemctl start coturn.service`

Should the domain and port be changed then the turn address in the `/assets/js/voiceChat.js` will
need to be modified in the `Lights-on-website` repositories to correctly reflect this change.

## Build and deploy the signaling server
As mentioned before, the signaling server did not have its deployment automated.
This is because it is a simple application to do one function and was heavily tested
before deployment therefore there was no benefit to automating the build process.

### Building
A `jar` file can be aquired by running `mvn compile package`

### Deploying
Simply copy the `target/quarkus-app/` directory into the `/opt/Lights-on-signalling/` directory on the server.

A systemd service is then created in the `/etc/systemd/system/lightsvoice.service` file:
```
[Unit]
Description=Lights On Voice server
StartLimitIntervalSec=0

[Service]
Type=simple
Restart=never
RestartSec=1
User=lights
ExecStart=java -jar /opt/Lights-on-signalling/quarkus-app/quarkus-run.jar
ExecStop=/bin/kill -s QUIT $MAINPID

[Install]
WantedBy=getty.target
```

The server will need Java installed on it.

The signaling server can then be started via systemd using `systemctl start lightsvoice.service`

## A deeper look into the CI/CD
### Lights-on-website
The website is built using the [Jekyll](https://jekyllrb.com/) static site generator.
The CI/CD script is main concerned with three steps:
1. Build the static site
2. Deploy the static site to the server
3. Notify Discord that the website has been updated

### Lights-on-game
The game is built using actions from the [Game CI project](https://game.ci)

There are two CI/CD workflows present in the `.github/workflows/` folder: `deploy.yml` and `testing.yml`

The `testing.yml` file is used for running the regression testing suite against new code changes and
reports any errors to the Discord server giving us clear notification an error has occured.

The `deploy.yml` is concerned about producing a WebGL WASM build which can then be deployed to the website.
This is deployed into a specific directory on the website (`/assets/unity`) which is how the two repositories are
merged togeter at the final moment. Another advantage of having seperate game and website repos is that it means
website updates are not stalled by having to rebuild the unity game as this is a lengthy process.
