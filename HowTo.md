Copy the dll to your custom tasks directory (usually Program Files/MsBuild).

Insert this line to import the custom task:


&lt;UsingTask AssemblyFile="C:\Program Files\MsBuild\FlexTask.dll" TaskName="BuildTask.Flex.FlexBuild" /&gt;



And to call the task:


&lt;FlexBuild WorkSpacePath="$(FlexWorkspace)" Configuration="Release" JavaHome="C:\Program Files\Java\jre1.6.0\_07" FlexSdkPath="C:\Flex3SDK" MetadataCreator="$(Creator)" MetadataDescription="$(Description)" MetadataPublisher="$(Publisher)" MetadataTitle="$(Title)"/&gt;



[Required](Required.md)
WorkspacePath => Path to the eclipse workspace that hold your flex project.

[Required](Required.md)
Configuration => Debug or Release

[Required](Required.md)
JavaHome => Path to the latest java runtime home

[Required](Required.md)
FlexSdkPath => Path to the flex sdk home. (Note, I had to copy the full framework to a folder without spaces,the compc tool failed in some circumstances with "Flex Builder 3 Plugin" folder.

[Optional](Optional.md)
OutputFile => Path to the final swf files (all swcs are compiled in their respective project bin folders)

[Optional](Optional.md)
EnableWarnings (True or False) => Enable strict warning (compilation stops if a warning is detected)

MetadataCreator, MetadataDescription, MetadataPublisher, MetadataTitle => Info embedded in the swf and swc files.

Note: This has only been tested on my flex project, not guaranteed to work on all projects.