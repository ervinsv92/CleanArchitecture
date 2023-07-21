
using CleanArchitecture.Data;
using CleanArchitecture.Domain;
using Microsoft.EntityFrameworkCore;

StreamerDbContext dbContext = new StreamerDbContext();

//await AddNewRecords();
//await QueryStreaming();
//await QueryFilter();
//await QueryMethods();
//await QueryLinq();
//await TrakingAndNotTraking();
//await AddNewStreamerWithVideo();
//await AddNewStreamerWithVideoId();
//await AddNewActorWithVideo();
//await AddNewDirectorWithVideo();
await MultipleEntityQuery();

Console.WriteLine("Presiones cualquier tecla para terminar el programa");
Console.ReadKey();

async Task MultipleEntityQuery() {
    //var videoWithActores = await dbContext.Videos.Include(q => q.Actores).FirstOrDefaultAsync(q => q.Id==1);
    //var actor = await dbContext!.Actor!.Select(q=>q.Nombre).ToListAsync();

    var videoWithDirector = await dbContext.Videos
        .Where(q=> q.Director != null)
        .Include(q => q.Director)
        .Select(q =>
            new
            {
                Director_Nombre_Completo = $"{q.Director.Nombre} - {q.Director.Apellido}",
                Movie = q.Nombre
            }
        ).ToListAsync();

    foreach ( var pelicula in videoWithDirector )
    {
        Console.WriteLine($"{pelicula.Movie} - {pelicula.Director_Nombre_Completo}");

    }
}
async Task AddNewDirectorWithVideo() { 

    var director = new Director { 
        Nombre="Lorenzo",
        Apellido="Basteri",
        VideoId=1
    };

    await dbContext.AddAsync(director);
    await dbContext.SaveChangesAsync();
}

async Task AddNewActorWithVideo()
{
    var actor = new Actor
    {
        Nombre = "Brad",
        Apellido = "Pitt"
    };

    await dbContext.AddAsync(actor);//al insertarse en la bd, de una vez se actualiza el objeto con el nuevo id, por eso el siguiente objeto de video actor ya lleva el id del actor listo
    await dbContext.SaveChangesAsync();

    var videoActor = new VideoActor { ActorId = actor.Id, VideoId = 1 };

    await dbContext.AddAsync(videoActor);
    await dbContext.SaveChangesAsync();
}

async Task AddNewStreamerWithVideo()
{
    var pantaya = new Streamer { Nombre = "Pantaya" };
    var hungerGames = new Video
    {
        Nombre = "Hunger Games",
        Streamer = pantaya
    };

    await dbContext.AddAsync(hungerGames);
    await dbContext.SaveChangesAsync();
}

async Task AddNewStreamerWithVideoId()
{
    
    var batmanForever = new Video
    {
        Nombre = "Batman Forever",
        StreamerId = 4
    };

    await dbContext.AddAsync(batmanForever);
    await dbContext.SaveChangesAsync();
}


async Task TrakingAndNotTraking() { 
    var streamerWithTraking = await dbContext.Streamers.FirstOrDefaultAsync(x => x.Id == 1); //traking esta activo por defecto
    var streamerWithNoTraking = await dbContext.Streamers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == 2);//as no traking, desliga el objeto de memoria, esto hace que no 
    //sirva para poder editar el dato en la BD, usar solo cuando se sabe que no se ocupa para modificar en la bd

    streamerWithTraking.Nombre = "Netflix super"; //se actualiza porque usa traking
    streamerWithNoTraking.Nombre = "Amazon prime plus"; //no se actualiza porque esta como no traking
    await dbContext.SaveChangesAsync();
}

async Task QueryLinq() {

    Console.WriteLine("Ingrese el servicio de streaming");
    var streamerNombre = Console.ReadLine();
    var streamers = await (from i in dbContext.Streamers
                           where EF.Functions.Like(i.Nombre, $"%{streamerNombre}%")
                           select i).ToListAsync();
    foreach (var streamer in streamers)
    {
        Console.WriteLine($"{streamer.Id} - {streamer.Nombre}");
    }
}

async Task QueryMethods() {
    var streamer = dbContext!.Streamers!;
    var firstAsync = await streamer.Where(y => y.Nombre.Contains("a")).FirstAsync(); //asume que existe la data, sino existe tira una excepcion (error)
    var firstOrDefaultAsync = await streamer.Where(y => y.Nombre.Contains("a")).FirstOrDefaultAsync(); //sino concuerda devuelve null, no tira exepcion de errores
    var firstOrDefault2 = await streamer.FirstOrDefaultAsync(y => y.Nombre.Contains("a")); //sino concuerda devuelve null, no tira exepcion de errores

    var singleAsync = await streamer.Where(x => x.Id==1).SingleAsync(); //si el resultado es mas de un registro tira una excepcion
    var singleOrDefaultAsync = await streamer.Where(x => x.Id == 1).SingleAsync(); //si el resultado es mas de un registro devuelve nulo

    var resultado = await streamer.FindAsync(1);//consulta por id
}

async Task QueryFilter()
{
    Console.WriteLine("Ingresa una compania de streaming");
    var streamingNombre = Console.ReadLine();
    var streamers = await dbContext!.Streamers!.Where(x => x.Nombre == streamingNombre).ToListAsync();

    foreach (var streamer in streamers)
    {
        Console.WriteLine($"{streamer.Id} - {streamer.Nombre}");
    }

    //var streamerPartialResults = await dbContext!.Streamers!.Where(x => x.Nombre.Contains(streamingNombre)).ToListAsync();
    var streamerPartialResults = await dbContext!.Streamers!.Where(x => EF.Functions.Like(x.Nombre, $"%{streamingNombre}%")).ToListAsync();

    foreach (var streamer in streamerPartialResults)
    {
        Console.WriteLine($"{streamer.Id} - {streamer.Nombre}");
    }
}

void QueryStreaming (){
    var streamers = dbContext!.Streamers!.ToList();
    foreach(var streamer in streamers)
    {
        Console.WriteLine($"{streamer.Id} - {streamer.Nombre}");
    }
}

async Task AddNewRecords() {
    Streamer streamer = new Streamer() { Nombre = "Disney", Url = "https://www.disney.com" };

    dbContext!.Streamers!.Add(streamer);

    await dbContext.SaveChangesAsync();

    var movies = new List<Video>
{
    new Video
    {
        Nombre="La Cenicienta",
        StreamerId=streamer.Id
    },
    new Video
    {
        Nombre="1001 Dalmatas",
        StreamerId=streamer.Id
    },
    new Video
    {
        Nombre="El Jorobado de Notredame",
        StreamerId=streamer.Id
    },
    new Video
    {
        Nombre="Star Wars",
        StreamerId=streamer.Id
    },
};

    await dbContext.AddRangeAsync(movies);
    await dbContext.SaveChangesAsync();
}