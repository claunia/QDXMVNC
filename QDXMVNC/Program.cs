using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace QDXMVNC
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("QDXMVNC - Quick & Dirty XBMC Music Video NFO Creator");
            Console.WriteLine("© 2014 Natalia Portillo");
            Console.WriteLine();

            if (args.Length != 1)
            {
                Console.WriteLine("This little tool recursively scans a folder searching for music video files and creates a XBMC .nfo containing info extracted from path and filename");
                Console.WriteLine("Expected structure is \"<Artist>/<Album>/<TrackNumber> <TrackName>.<Extension>\"");
                Console.WriteLine("Supported extensions are: mp4, m4v, avi, mkv, mpg, ts, ps, vob, ogm, divx, asf");

                return;
            }

            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("Given argument is not a directory or does not exist");
                return;
            }

            List<string> artist_paths = new List<string>(Directory.EnumerateDirectories(args[0]));

            StringBuilder nfo_sb = new StringBuilder();
            int skipped = 0;
            int written = 0;
            int videos = 0;
            int albums = 0;

            foreach (string artist_path in artist_paths)
            {
                string artist = Path.GetFileName(artist_path);

                List<string> album_paths = new List<string>(Directory.EnumerateDirectories(artist_path));

                foreach (string album_path in album_paths)
                {
                    string album = Path.GetFileName(album_path);

                    List<string> video_files = new List<string>(Directory.EnumerateFiles(album_path));

                    foreach (string video_file in video_files)
                    {
                        string video_without_extension = Path.GetFileNameWithoutExtension(video_file);
                        string video_extension = Path.GetExtension(video_file);

                        string[] video_pieces = video_without_extension.Split(new char [] {' '}, 2);

                        if (video_extension.ToLower() == ".mp4" || video_extension.ToLower() == ".m4v" || video_extension.ToLower() == ".avi" ||
                            video_extension.ToLower() == ".mkv" || video_extension.ToLower() == ".mpg" || video_extension.ToLower() == ".ts" ||
                            video_extension.ToLower() == ".ps" || video_extension.ToLower() == ".vob" || video_extension.ToLower() == ".ogm" ||
                            video_extension.ToLower() == ".divx" || video_extension.ToLower() == ".asf")
                        {
                            string nfo_path_filename = album_path + Path.DirectorySeparatorChar + video_without_extension + ".nfo";

                            if(File.Exists(nfo_path_filename))
                            {
                                Console.WriteLine("{0} exists, skipping.", nfo_path_filename);
                                skipped++;
                            }
                            else
                            {
                                nfo_sb.AppendLine("<musicvideo>");
                                nfo_sb.AppendFormat("\t<album>{0}</album>", System.Security.SecurityElement.Escape(album)).AppendLine();
                                nfo_sb.AppendFormat("\t<artist>{0}</artist>", System.Security.SecurityElement.Escape(artist)).AppendLine();
                                nfo_sb.AppendFormat("\t<title>{0}</title>", System.Security.SecurityElement.Escape(video_pieces[1])).AppendLine();
                                nfo_sb.AppendFormat("\t<track>{0}</track>", System.Security.SecurityElement.Escape(video_pieces[0])).AppendLine();
                                nfo_sb.AppendLine("</musicvideo>");

                                Console.WriteLine("Writing {0}", nfo_path_filename);
                                File.WriteAllText(nfo_path_filename, nfo_sb.ToString(), Encoding.UTF8);
                                written++;
                            }

                            videos++;
                        }
                    }
                }

                List<string> non_album_files = new List<string>(Directory.EnumerateFiles(artist_path));

                foreach (string video_file in non_album_files)
                {
                    string video_without_extension = Path.GetFileNameWithoutExtension(video_file);
                    string video_extension = Path.GetExtension(video_file);

                    string[] video_pieces = video_without_extension.Split(new char [] {' '}, 2);

                    if (video_extension.ToLower() == ".mp4" || video_extension.ToLower() == ".m4v" || video_extension.ToLower() == ".avi" ||
                        video_extension.ToLower() == ".mkv" || video_extension.ToLower() == ".mpg" || video_extension.ToLower() == ".ts" ||
                        video_extension.ToLower() == ".ps" || video_extension.ToLower() == ".vob" || video_extension.ToLower() == ".ogm" ||
                        video_extension.ToLower() == ".divx" || video_extension.ToLower() == ".asf")
                    {
                        string nfo_path_filename = artist_path + Path.DirectorySeparatorChar + video_without_extension + ".nfo";

                        if(File.Exists(nfo_path_filename))
                        {
                            Console.WriteLine("{0} exists, skipping.", nfo_path_filename);
                            skipped++;
                        }
                        else
                        {
                            nfo_sb.AppendLine("<musicvideo>");
                            nfo_sb.AppendFormat("\t<album>{0}</album>", System.Security.SecurityElement.Escape("[non-album tracks]")).AppendLine();
                            nfo_sb.AppendFormat("\t<artist>{0}</artist>", System.Security.SecurityElement.Escape(artist)).AppendLine();
                            nfo_sb.AppendFormat("\t<title>{0}</title>", System.Security.SecurityElement.Escape(video_pieces[1])).AppendLine();
                            nfo_sb.AppendFormat("\t<track>{0}</track>", System.Security.SecurityElement.Escape(video_pieces[0])).AppendLine();
                            nfo_sb.AppendLine("</musicvideo>");

                            Console.WriteLine("Writing {0}", nfo_path_filename);
                            File.WriteAllText(nfo_path_filename, nfo_sb.ToString(), Encoding.UTF8);
                            written++;
                        }

                        videos++;
                    }
                }
                albums += album_paths.Count;
            }

            Console.WriteLine("{0} music videos from {1} artists in {2} albums", videos, artist_paths.Count, albums);
            Console.WriteLine("{0} NFOs written", written);
            Console.WriteLine("{0} NFOs skipped", skipped);
        }
    }
}
