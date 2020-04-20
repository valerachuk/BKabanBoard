async function processApi(url, method = "GET", body = "", onOk = console.log, onError = console.log) {
    let response = await fetch(url, {
        method: method,
        body: body,
        headers:{
            'Content-Type': 'application/json'
        }
    });

    if(response.ok)
        onOk(await response.text());
    else
        onError(await response.text());
}