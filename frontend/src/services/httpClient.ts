
async function get<TResponse>(url: string): Promise<{data:TResponse}>{
    const response = await fetch('https://localhost:5001' + url);
    const data = await response.json();
    return data;
}

async function post<TResponse, TBody>(url:string, body: TBody): Promise<{data: TResponse}>{
    const response = await fetch('https://localhost:5001'+url, {
        method: "POST",
        headers:{
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
    })

    const data = await response.json();
    return data;
}

export const httpClient ={get, post}