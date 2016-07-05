# Wox.Plugins
[![Build Status](https://travis-ci.org/Shinao/Wox.Plugins.svg?branch=master)](https://travis-ci.org/Shinao/Wox.Plugins)

Plugins for [Wox](https://github.com/Wox-launcher/Wox)
- Need : save and retrieve infos easily
- Ahk : run autohotkey script from the directory

<br>
**Preview [Need]**
![Debugging gesture](/docs/Wox.Need.gif)

<br>
**How To Use [Need]**

| What        | How           |
| ------------- |-------------|
| Get to clipboard        | `need Key` |
| Save        | `need KeyWithoutSpaces Value` |
| Delete      | `need delete Key`      |
| Open file | `need open`      |
| Reload file | `need reload`     |

<br>
**For example :**
- Save server_ip first : `need server_ip 127.0.0.1`
- `need server_ip` : 127.0.0.1 is now in the clipboard
- finally delete it : `need delete server_ip`
