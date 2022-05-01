import { API, CampaignMedia, CampaignMediaType } from 'sdk'

export class CampaignMediaService {
	static get(id: number) {
		return API.get<CampaignMedia>(`/media/${id}`)
	}

	static getAll() {
		return API.get<Array<CampaignMedia>>('/media')
	}

	static extractUriByType(content: string, type: CampaignMediaType) {
		switch (type) {
			case CampaignMediaType.File:
				return content
			case CampaignMediaType.YouTube:
				return CampaignMediaService.extractYouTubeVideoId(content)
			case CampaignMediaType.Twitch:
				return CampaignMediaService.extractTwitchVideoId(content)
		}
	}

	private static extractYouTubeVideoId(content: string) {
		const regex = /^.*(youtu\.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*/
		const videoId = content.match(regex)?.[2] as string | undefined
		if (videoId?.length !== 11) {
			throw new Error('Invalid YouTube URI')
		}
		return videoId
	}

	private static extractTwitchVideoId(content: string) {
		const regex = /^.*(twitch\.tv\/videos\/)([^#\&\?]*).*/
		const videoId = content.match(regex)?.[2] as string | undefined
		if (videoId?.length !== 10) {
			throw new Error('Invalid Twitch URI')
		}
		return videoId
	}

	static save(media: CampaignMedia) {
		return API.post<CampaignMedia>('/media', media)
	}

	static delete(id: number) {
		return API.delete(`/media/${id}`)
	}
}