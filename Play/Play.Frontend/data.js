window.onload = () =>
{

    fetch("https://localhost:5001/api/items")
        .then(response =>
        {
            return response.json();
        })
        .then(responseResult =>
        {
            console.log(responseResult);
        })
        .catch(err =>
        {
            console.log(origin);
            console.log(err);
        })

    fetch("https://localhost:6001/api/Items?userId=3fa85f64-5717-4562-b3fc-2c963f66afa6")
        .then(response =>
        {
            return response.json();
        })
        .then(responseResult =>
        {
            console.log(responseResult)
        })
        .catch(err =>
        {
            console.log(err);
        })
};