﻿namespace Depra.Assets.Runtime.Files.Idents
{
    public interface IAssetIdent
    {
        public string Uri { get; }
    }

    public interface IAssetWIthExtension : IAssetIdent
    {
        public string Extension { get; }
        
        public string NameWithExtension { get; }
    }
    
    // public interface IAssetIdent
    // {
    //     public string Name { get; }
    //
    //     public string Extension { get; }
    //
    //     public string AbsolutePath { get; }
    //
    //     public string AbsoluteDirectoryPath { get; }
    //
    //     public string RelativePath { get; }
    // }
}