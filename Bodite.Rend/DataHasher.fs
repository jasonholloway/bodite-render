module DataHasher

   let MD5Hash (input : byte[]) =
      use md5 = System.Security.Cryptography.MD5.Create()
      input
      |> md5.ComputeHash
      |> Seq.map (fun c -> c.ToString("X2"))
      |> Seq.reduce (+)   
