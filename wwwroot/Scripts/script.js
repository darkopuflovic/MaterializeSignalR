var lastSize;
var myAvatar;
var myUser;
var myColor;
var myTextColor;
var rootPath;
var myid;
var toId;

NodeList.prototype.forEach = Array.prototype.forEach;

document.addEventListener('DOMContentLoaded', function() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(rootPath + "/ChatHub")
        .build();

    connection.on("ReceiveMessage", function (fromId, message) {
        var el = null;
        document.querySelectorAll("#slide-out>.li-user").forEach(function(item) {
            if (item.getAttribute("data-id") === fromId) {
                el = item;
            }
        });;
        
        if (el !== null && el !== undefined) {
            var name = el.childNodes[0].childNodes[1].innerText;
            var color = el.childNodes[0].childNodes[2].value;
            var textColor = el.childNodes[0].childNodes[3].value;

            if (name !== null && name !== undefined && color !== null && color !== undefined && textColor !== null && textColor !== undefined) {
                addMessage(name, color, textColor, message, "otherChat");
            }
        }
    });

    connection.on("ClientConnected", function (client) {
        var id = client["id"];
        var avatar = client["avatar"];
        var color = client["color"];
        var textColor = client["textColor"];
        var userName = client["userName"];

        if (myid !== null && myid !== undefined && id !== null && id !== undefined) {
            if (myid.toUpperCase() !== id.toUpperCase()) {
                addAvailableUser(id, userName, avatar, color, textColor);
            }
        }
    });

    connection.on("ClientDisconnected", function (id) {
        removeAvailableUser(id);
    });

    connection.on("YourConnectionID", function (id) {
        myid = id;
    });

    var sendButton = document.getElementById("sendButton");
    var messageInput = document.getElementById("message");
    
    if (sendButton !== null && sendButton !== undefined) {
        sendButton.addEventListener("click", function (event) {
            event.preventDefault();
            
            const message = document.getElementById("message").value;

            if (message !== null && message !== undefined && message !== "" && toId !== null && toId !== undefined) {
                connection.invoke("SendMessage", toId, myid, message)
                .then(function() {
                    addMessage(myUser, myColor, myTextColor, message, "myChat");
                })
                .catch(function(err) {
                    console.error(err.toString());
                });
            }

            messageInput.value = "";
            M.updateTextFields();
        });
    }

    if (messageInput !== null && messageInput !== undefined) {
        messageInput.addEventListener("keyup", function (event) {
            event.preventDefault();
            
            if (event.keyCode === 13) {
                const message = document.getElementById("message").value;

                if (message !== null && message !== undefined && message !== "" && toId !== null && toId !== undefined) {
                    connection.invoke("SendMessage", toId, myid, message)
                    .then(function() {
                        addMessage(myUser, myColor, myTextColor, message, "myChat");
                    })
                    .catch(function(err) {
                        console.error(err.toString());
                    });
                }

                messageInput.value = "";
            }
        });
    }

    connection.start().then(function() {
        connection.invoke("GetClients").then(function(clients) {
            for (var key in clients) {
                if (clients[key] !== null && clients[key] !== undefined) {
                    var id = clients[key]["id"];
                    var avatar = clients[key]["avatar"];
                    var color = clients[key]["color"];
                    var textColor = clients[key]["textColor"];
                    var userName = clients[key]["userName"];

                    if (myid.toUpperCase() !== id.toUpperCase()) {
                        addAvailableUser(id, userName, avatar, color, textColor);
                    }
                }
            }
        }).catch(function(err) {
            console.error(err.toString());
        });
    }).catch(function(err) {
        console.error(err.toString());
    });
});

function setUsernameAndColor(userName, userColor, userTextColor) {
    myUser = userName;
    myColor = userColor;
    myTextColor = userTextColor;
}

function setRootPath(root) {
    rootPath = root;
}

document.addEventListener('DOMContentLoaded', function() {
    var elems = document.querySelectorAll('.sidenav');
    var sidenavs = M.Sidenav.init(elems, { edge: "left", draggable: true });

    var elems2 = document.querySelectorAll('.modal');
    var modals = M.Modal.init(elems2);

    var li = document.querySelectorAll(".li-user");
    li.forEach(function(item) {
        item.addEventListener("mousedown", function(event) {
            if (event.button === 0 || event.button === 1) {
                item.style.color = "white";
            }
        });

        item.addEventListener("mouseup", function(event) {
            item.style = null;
        });
    });

    var matCol0 = document.getElementById("materialColor0");

    if (matCol0 !== null && matCol0 !== undefined) {
        matCol0.checked = true;
    }
});

window.addEventListener("resize", function() {
    if (window.innerWidth > 992 && lastSize <= 992) {
        var elem = document.querySelector(".sidenav");
        var instance = M.Sidenav.getInstance(elem);
        instance.close();
    }

    lastSize = window.innerWidth;

    var scroll = document.getElementById("messages-scroll");

    if (scroll !== null && scroll !== undefined) {
        scroll.scrollTo(0, scroll.scrollHeight - scroll.clientHeight);
    }
});

function avatarImage(e) {
    var avatarImage = document.getElementById('avatar-image');

    if (e.target.files.length > 0 && avatarImage !== null && avatarImage !== undefined) {
        var src = URL.createObjectURL(e.target.files[0]);
        avatarImage.src = src;
    }
}

function setAvatar() {
    var userAvatarImage = document.getElementById("userAvatarImage");

    if (userAvatarImage !== null && userAvatarImage !== undefined) {
        userAvatarImage.src = myAvatar;
    }
}

function setName() {
    var name = document.getElementById("name").value;
}

function setNameOnEnter(e) {
    if (e.keyCode === 13) {
        setName();
    }
}

function setEmail() {
    var email = document.getElementById("email");
    
    if (email !== null && email !== undefined) {
        email.checkValidity();
    }
}

function setPassword() {
    var password = document.getElementById("password");

    if (password !== null && password !== undefined) {
        password.checkValidity();
    }
}

function setEmailOnEnter(e) {
    if (e.keyCode === 13) {
        setEmail();
    }
}

function setPasswordOnEnter(e) {
    if (e.keyCode === 13) {
        setPassword();
    }
}

function clearColors() {
    document.querySelectorAll(".color-palette-wrapper").forEach(function(item) {
        item.classList.remove("js-active");
    });
}

function showColors(id) {
    clearColors();
    var idEl = document.getElementById(id);

    if (idEl !== null && idEl !== undefined) {
        idEl.classList.add("js-active");
    }
}

function rgbGetValues(rgb) {
    return rgb.replace("rgb(", "").replace(")", "").split(',').map(function(i) { return i.trim(); });
}

function rgb2hex(rgb) {
    var red = rgb[0];
    var green = rgb[1];
    var blue = rgb[2];
    var rgbVal = blue | (green << 8) | (red << 16);
    return '#' + (0x1000000 + rgbVal).toString(16).slice(1).toUpperCase();
}

function colorChanged(rgb, color) {
    var hex = rgb2hex(rgbGetValues(rgb));
    var userColor = document.getElementById("userColor");
    var userColorImage = document.getElementById("userColorImage");
    var colorHidden = document.getElementById("colorHidden");
    var textColorHidden = document.getElementById("textColorHidden");

    if (userColor !== null && userColor !== undefined) {
        userColor.style.backgroundColor = hex;
    }

    if (userColorImage !== null && userColorImage !== undefined) {
        userColorImage.style.color = color;
    }

    if (colorHidden !== null && colorHidden !== undefined) {
        colorHidden.value = hex;
    }

    if (textColorHidden !== null && textColorHidden !== undefined) {
        textColorHidden.value = color;
    }

    var elem = document.querySelector("#colorPickerModal");
    var instance = M.Modal.getInstance(elem);
    instance.close();
}

function addAvailableUser(id, name, avatar, color, textColor) {
    var list = document.getElementById("slide-out");

    if (list !== null && list !== undefined) {
        var li = document.createElement("li");
        li.setAttribute("data-id", id);
        li.className = "waves-effect waves-light btn li-user grey lighten-4 waves-indigo";
        var divOutside = document.createElement("div");
        divOutside.className = "row mb-0 non-selectable";
        var divAvatar = document.createElement("div");
        divAvatar.className = "col s3";
        var img = document.createElement("img");
        img.src = rootPath + "/" + avatar;
        img.className = "circle image-avatar-list";
        var divName = document.createElement("div");
        divName.className = "col s9 overflow-text-ellipsis";
        divName.innerText = name;
        divAvatar.appendChild(img);
        divOutside.appendChild(divAvatar);
        divOutside.appendChild(divName);
        var colorEl = document.createElement("input");
        var textColorEl = document.createElement("input");
        colorEl.setAttribute("name", "color");
        textColorEl.setAttribute("name", "textColor");
        colorEl.setAttribute("type", "hidden");
        textColorEl.setAttribute("type", "hidden");
        colorEl.setAttribute("value", color);
        textColorEl.setAttribute("value", textColor);
        divOutside.appendChild(colorEl);
        divOutside.appendChild(textColorEl);
        li.appendChild(divOutside);
        list.appendChild(li);
        li.addEventListener("click", function() {
            document.querySelectorAll("#slide-out>.li-user").forEach(function(item) {
                item.classList.remove("blue")
                item.classList.remove("darken-1");
                item.classList.add("grey");
                item.classList.add("lighten-4");
                toId = null;
            });

            this.classList.remove("grey");
            this.classList.remove("lighten-4");
            this.classList.add("blue");
            this.classList.add("darken-1");
            toId = this.getAttribute("data-id");

            if (toId !== null && toId !== undefined) {
                document.getElementById("message").removeAttribute("disabled");
                document.getElementById("sendButton").removeAttribute("disabled");
            }
            else {
                document.getElementById("message").setAttribute("disabled", "disabled");
                document.getElementById("sendButton").setAttribute("disabled", "disabled");
            }
        });
    }
}

function removeAvailableUser(id) {
    document.querySelectorAll("#slide-out>.li-user").forEach(function(item) {
        if (item !== null && item !== undefined) {
            if (item.getAttribute("data-id") === id) {
                item.remove();

                if (id === toId) {
                    toId = null;
                }

                if (toId !== null && toId !== undefined) {
                    document.getElementById("message").removeAttribute("disabled");
                    document.getElementById("sendButton").removeAttribute("disabled");
                }
                else {
                    document.getElementById("message").setAttribute("disabled", "disabled");
                    document.getElementById("sendButton").setAttribute("disabled", "disabled");
                }
            }
        }
    });
}

function addMessage(name, color, textColor, message, className) {
    var scroll = document.getElementById("messages-scroll");

    if (scroll !== null && scroll !== undefined) {
        var bottom = scroll.scrollTop + scroll.clientHeight === scroll.scrollHeight;
    }

    var chat = document.getElementById("chatMessages");

    var li = document.createElement("li");
    li.classList.add(className);
    li.style.color = textColor;
    li.style.backgroundColor = color;
    var p = document.createElement("p");
    p.innerText = message;
    li.appendChild(p);
    var small = document.createElement("small");
    small.classList.add("smallClass");
    small.innerText = name;
    li.appendChild(small);
    chat.appendChild(li);

    if (bottom && scroll !== null && scroll !== undefined) {
        scroll.scrollTo(0, scroll.scrollHeight - scroll.clientHeight);
    }
}