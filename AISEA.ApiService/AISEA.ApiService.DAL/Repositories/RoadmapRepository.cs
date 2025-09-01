using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class RoadmapRepository : GenericRepository<StudyRoadMap>
    {
        public RoadmapRepository(AiseaContext context) : base(context)
        {
        }

        // Create roadmap
        public async Task<StudyRoadMap> CreateRoadmapAsync(long studentId, string name)
        {
            var roadmap = new StudyRoadMap
            {
                Name = name,
                StudentProfileId = studentId
            };

            _context.StudyRoadMaps.Add(roadmap);
            await _context.SaveChangesAsync();
            return roadmap;
        }

        // Create node
        public async Task<StudyRoadMapNode> CreateNodeAsync(long roadmapId, StudyRoadMapNode node)
        {
            node.StudyRoadMapId = roadmapId;
            _context.StudyRoadMapNodes.Add(node);
            await _context.SaveChangesAsync();
            return node;
        }

        // Delete a single node with all associated links
        public async Task<bool> DeleteNodeAsync(long nodeId)
        {
            var node = await _context.StudyRoadMapNodes
                .Include(n => n.Prerequisites)
                .Include(n => n.Dependents)
                .FirstOrDefaultAsync(n => n.Id == nodeId);

            if (node == null) return false;

            // Remove all related links (de-duplicated)
            var linksToRemove = node.Prerequisites
                                    .Concat(node.Dependents)
                                    .Distinct()
                                    .ToList();

            _context.StudyRoadMapNodeLinks.RemoveRange(linksToRemove);
            _context.StudyRoadMapNodes.Remove(node);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoadmapAsync(long roadmapId)
        {
            var roadmap = await _context.StudyRoadMaps
                .Include(r => r.Nodes)
                .FirstOrDefaultAsync(r => r.Id == roadmapId);

            if (roadmap == null) return false;

            // Delete all links associated with nodes in the roadmap
            var nodeIds = roadmap.Nodes.Select(n => n.Id).ToList();
            var linksToDelete = await _context.StudyRoadMapNodeLinks
                .Where(l => nodeIds.Contains(l.FromNodeId) || nodeIds.Contains(l.ToNodeId))
                .ToListAsync();

            _context.StudyRoadMapNodeLinks.RemoveRange(linksToDelete);
            _context.StudyRoadMapNodes.RemoveRange(roadmap.Nodes);
            _context.StudyRoadMaps.Remove(roadmap);

            await _context.SaveChangesAsync();
            return true;
        }

        // Get roadmap graph
        public async Task<StudyRoadMap?> GetRoadmapWithGraphAsync(long roadmapId)
        {
            return await _context.StudyRoadMaps
                .Include(r => r.Nodes)
                    .ThenInclude(n => n.Prerequisites)
                        .ThenInclude(l => l.FromNode)
                .Include(r => r.Nodes)
                    .ThenInclude(n => n.Dependents)
                        .ThenInclude(l => l.ToNode)
                .FirstOrDefaultAsync(r => r.Id == roadmapId);
        }

        public async Task<StudyRoadMapNodeLink?> CreateLinkAsync(long fromNodeId, long toNodeId)
        {
            var fromNode = await _context.StudyRoadMapNodes.FindAsync(fromNodeId);
            var toNode = await _context.StudyRoadMapNodes.FindAsync(toNodeId);

            if (fromNode == null || toNode == null) return null;

            var link = new StudyRoadMapNodeLink
            {
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId
            };

            _context.StudyRoadMapNodeLinks.Add(link);
            await _context.SaveChangesAsync();

            return link;
        }

        public async Task<bool> DeleteLinkAsync(long linkId)
        {
            var link = await _context.StudyRoadMapNodeLinks.FindAsync(linkId);
            if (link == null) return false;

            _context.StudyRoadMapNodeLinks.Remove(link);
            await _context.SaveChangesAsync();
            return true;
        }


        // Get a single node by id (no links)
        public async Task<StudyRoadMapNode?> GetNodeByIdAsync(long nodeId)
        {
            return await _context.StudyRoadMapNodes
                .AsNoTracking() // No tracking, safe for read-only
                .FirstOrDefaultAsync(n => n.Id == nodeId);
        }

        // Update node data (not links)
        public async Task<StudyRoadMapNode?> UpdateNodeAsync(StudyRoadMapNode node)
        {
            var existing = await _context.StudyRoadMapNodes.FindAsync(node.Id);
            if (existing == null) return null;

            existing.SubjectCode = node.SubjectCode;
            existing.SemesterNumber = node.SemesterNumber;
            existing.SubjectName = node.SubjectName;
            existing.Description = node.Description;
            existing.IsInternalSubjectData = node.IsInternalSubjectData;

            await _context.SaveChangesAsync();
            return existing;
        }


        public async Task<bool> ReplaceNodesInRoadmapAsync(long roadmapId, List<StudyRoadMapNode> newNodes)
        {
            var roadmap = await _context.StudyRoadMaps
                .Include(r => r.Nodes)
                .FirstOrDefaultAsync(r => r.Id == roadmapId);

            if (roadmap == null) return false;

            // Delete all existing nodes and associated links
            var existingNodeIds = roadmap.Nodes.Select(n => n.Id).ToList();
            var linksToDelete = await _context.StudyRoadMapNodeLinks
                .Where(l => existingNodeIds.Contains(l.FromNodeId) || existingNodeIds.Contains(l.ToNodeId))
                .ToListAsync();

            _context.StudyRoadMapNodeLinks.RemoveRange(linksToDelete);
            _context.StudyRoadMapNodes.RemoveRange(roadmap.Nodes);

            // Insert new nodes
            newNodes.ForEach(n => n.StudyRoadMapId = roadmapId);
            _context.StudyRoadMapNodes.AddRange(newNodes);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddLinksToRoadmapAsync(List<(long FromNodeId, long ToNodeId)> links)
        {
            if (links == null || !links.Any())
                return true;

            // Filter out invalid links where nodes do not exist
            var nodeIds = links.SelectMany(l => new[] { l.FromNodeId, l.ToNodeId }).Distinct().ToList();
            var existingNodeIds = await _context.StudyRoadMapNodes
                .Where(n => nodeIds.Contains(n.Id))
                .Select(n => n.Id)
                .ToListAsync();

            var validLinks = links
                .Where(l => existingNodeIds.Contains(l.FromNodeId) && existingNodeIds.Contains(l.ToNodeId))
                .Select(l => new StudyRoadMapNodeLink
                {
                    FromNodeId = l.FromNodeId,
                    ToNodeId = l.ToNodeId
                }).ToList();

            if (!validLinks.Any()) return true;

            _context.StudyRoadMapNodeLinks.AddRange(validLinks);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<long> GetIDByStudentProfileIDAsync(long studentProfileId)
        {
            var roadmap = await _context.StudyRoadMaps.FirstOrDefaultAsync(r => r.StudentProfileId == studentProfileId);

            if (roadmap is null) return -1;
            return roadmap.Id;
        }
    }
}